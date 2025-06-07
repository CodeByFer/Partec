using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;
using SkiaSharp;

namespace Partec.MVVM
{
    public class MVGraficas : MVBaseCRUD<Incidencia>
    {
        private readonly IncidenciaServicio _incidenciaServicio;

        public MVGraficas(GestionincidenciasContext contexto)
        {
            contexto.Database.OpenConnection();
            _incidenciaServicio = new IncidenciaServicio(contexto);
            SeriesPorEstado = new ObservableCollection<ISeries>();
            SeriesPorDepartamento = new ObservableCollection<ISeries>();
            SeriesPorTipo = new ObservableCollection<ISeries>();
            SeriesPorMes = new ObservableCollection<ISeries>();
        }

        public ObservableCollection<ISeries> SeriesPorEstado { get; private set; }
        public ObservableCollection<ISeries> SeriesPorDepartamento { get; private set; }
        public ObservableCollection<ISeries> SeriesPorTipo { get; private set; }
        public ObservableCollection<ISeries> SeriesPorMes { get; private set; }
        public Axis[] LabelsMeses { get; private set; } // Etiquetas para eje X (opcional)
        public Axis[] LabelDepartamento { get; private set; } // Etiquetas para eje X (opcional)
        public Axis[] YAxisDepartamento { get; private set; }

        private String[] todosLosMeses =
        {
            "Enero",
            "Febrero",
            "Marzo",
            "Abril",
            "Mayo",
            "Junio",
            "Julio",
            "Agosto",
            "Septiembre",
            "Octubre",
            "Noviembre",
            "Diciembre",
        };

        public async Task CargarDatosAsync()
        {
            try
            {
                var incidencias = (await _incidenciaServicio.GetAllAllAsync()).ToList();

                // === Gráfico de Pastel por Estado ===
                var porEstado = incidencias
                    .GroupBy(i => i.IdEstadoNavigation.DescripcionEstado)
                    .Select(g => new { Estado = g.Key, Total = g.Count() })
                    .ToList();

                SeriesPorEstado.Clear();
                foreach (var item in porEstado)
                {
                    SeriesPorEstado.Add(
                        new PieSeries<double>
                        {
                            Values = new[] { (double)item.Total },

                            Name = item.Estado,
                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                            DataLabelsSize = 24,
                            DataLabelsPaint = new SolidColorPaint(SKColors.White),
                            HoverPushout = 10,
                            IsVisible = true,
                            Tag = item.Estado,
                        }
                    );
                }

                // === Gráfico de Barras por Departamento ===
                var porDepto = incidencias
                    .GroupBy(i => i.IdDepartamentoNavigation.Nombre)
                    .Select(g => new { Departamento = g.Key, Total = g.Count() })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                SeriesPorDepartamento.Clear();
                foreach (var item in porDepto)
                {
                    SeriesPorDepartamento.Add(
                        new ColumnSeries<double>
                        {
                            Values = new[] { (double)item.Total },
                            Name = item.Departamento,

                            DataLabelsSize = 24,
                            DataLabelsPaint = new SolidColorPaint(SKColors.White),
                            DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,

                            Tag = item.Departamento,
                        }
                    );
                }

                // === Gráfico de Pastel por Tipo ===
                var porTipo = incidencias
                    .GroupBy(i => i.TipoIncidencia)
                    .Select(g => new { Tipo = g.Key, Total = g.Count() })
                    .ToList();

                SeriesPorTipo.Clear();
                foreach (var item in porTipo)
                {
                    SeriesPorTipo.Add(
                        new PieSeries<double>
                        {
                            Values = new[] { (double)item.Total },
                            Name = item.Tipo,

                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                            DataLabelsSize = 24,
                            DataLabelsPaint = new SolidColorPaint(SKColors.White),
                            HoverPushout = 10,
                            IsVisible = true,
                            Tag = item.Tipo,
                        }
                    );
                }
                // 1. Obtener el rango de fechas de las incidencias
                var fechaMin = incidencias.Min(i => i.FechaIncidencia).Date;
                var fechaMax = incidencias.Max(i => i.FechaIncidencia).Date;

                // 2. Generar todos los meses del rango
                var mesesCompletos = Enumerable
                    .Range(
                        0,
                        ((fechaMax.Year - fechaMin.Year) * 12) + fechaMax.Month - fechaMin.Month + 1
                    )
                    .Select(i => new DateTime(fechaMin.Year, fechaMin.Month, 1).AddMonths(i))
                    .ToList(); // Lista de primeros días de cada mes

                // 3. Agrupar incidencias por mes
                var agrupado = incidencias
                    .GroupBy(i => new { i.FechaIncidencia.Year, i.FechaIncidencia.Month })
                    .ToDictionary(g => new DateTime(g.Key.Year, g.Key.Month, 1), g => g.Count());

                // 4. Combinar todos los meses con los datos agrupados
                var porMes = mesesCompletos
                    .Select(mes => new
                    {
                        Mes = mes.ToString("MMMM yyyy", new CultureInfo("es-ES")),
                        Total = agrupado.TryGetValue(mes, out int count) ? count : 0,
                    })
                    .ToList();

                //var porMes = incidencias
                //    .GroupBy(i => new { i.FechaIncidencia.Year, i.FechaIncidencia.Month })
                //    .OrderBy(g => g.Key.Year)
                //    .ThenBy(g => g.Key.Month)
                //    .Select(g => new
                //    {
                //        Mes = $"{g.Key.Month:00}/{g.Key.Year}", // ej. "04/2025"
                //        Total = g.Count(),
                //    })
                //    .ToList();

                SeriesPorMes.Clear();
                SeriesPorMes.Add(
                    new ColumnSeries<double>
                    {
                        Values = porMes.Select(x => (double)x.Total).ToArray(),
                        DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                        DataLabelsSize = 24,
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),

                        Name = "Incidencias este mes: ",
                        Tag = "Incidencias por mes",
                        IsVisible = true,
                    }
                );

                // Etiquetas para eje X
                LabelsMeses = new Axis[]
                {
                    new Axis { Labels = porMes.Select(x => x.Mes).ToList(), LabelsRotation = 45 },
                };

                List<String> departamentos = porDepto.Select(x => x.Departamento).ToList();

                LabelDepartamento = new Axis[]
                {
                    new Axis
                    {
                        Labels = [""],
                        LabelsRotation = 90,
                        LabelsPaint = new SolidColorPaint { Color = SKColors.Black },
                    },
                };

                YAxisDepartamento = new Axis[]
                {
                    new Axis { MinLimit = 0, Name = "Cantidad" },
                };
                OnPropertyChanged(nameof(YAxisDepartamento));
                OnPropertyChanged(nameof(SeriesPorMes));
                OnPropertyChanged(nameof(LabelDepartamento));
                OnPropertyChanged(nameof(LabelsMeses));

                // Notificar cambio
                OnPropertyChanged(nameof(SeriesPorEstado));
                OnPropertyChanged(nameof(SeriesPorDepartamento));
                OnPropertyChanged(nameof(SeriesPorTipo));
            }
            catch (Exception ex)
            {
                // Aquí puedes loguear o mostrar mensaje al usuario si deseas
                Console.WriteLine($"Error al cargar gráficos: {ex.Message}");
            }
        }

        private void CreateImageFromCartesianControl(CartesianChart obj)
        {
            var skChart = new SKCartesianChart(obj) { Width = 900, Height = 600 };
            skChart.SaveImage("CartesianImageFromControl.png");
            skChart.GetImage();
        }

        private void CreateImageFromPieControl(PieChart obj)
        {
            var skChart = new SKPieChart(obj) { Width = 900, Height = 600 };
            skChart.SaveImage("PieImageFromControl.png");
        }

        private void CreateImageFromGeoControl(GeoMap obj)
        {
            var skChart = new SKGeoMap(obj) { Width = 900, Height = 600 };
            skChart.SaveImage("MapImageFromControl.png");
        }
    }
}
