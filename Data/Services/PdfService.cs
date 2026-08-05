using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using EscuelaManagement.Data.Models;
using System.Collections.Generic;
using System;

namespace EscuelaManagement.Data.Services
{
    public class PdfService
    {
        // ===================================================
        // 1. GENERAR LISTA DE ASISTENCIA MENSUAL (31 DÍAS)
        // ===================================================
        public byte[] GenerarListaAsistencia(List<Student> alumnos, string nombreCurso, string mesSeleccionado, string nombreEscuela = "EscuelaManager", string logoBase64 = "")
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Formato Horizontal para que quepan los 31 días
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Black));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var cleanLogo = logoBase64.Contains(',') ? logoBase64.Split(',')[1] : logoBase64;
                                    row.ConstantItem(50).Height(50).Image(Convert.FromBase64String(cleanLogo));
                                }
                                catch { /* Ignorar si falla */ }
                            }

                            row.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().AlignCenter().Text(nombreEscuela.ToUpper()).Bold().FontSize(14).FontColor("#003366");
                                c.Item().AlignCenter().Text("CONTROL DE ASISTENCIA MENSUAL").Bold().FontSize(11).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text(t => {
                                t.Span("CURSO ASIGNADO: ").Bold().FontColor("#003366").FontSize(10);
                                t.Span(nombreCurso.ToUpper()).Bold().FontSize(10);
                            });
                            row.ConstantItem(200).AlignRight().Text(t => {
                                t.Span("MES: ").Bold().FontColor("#003366").FontSize(10);
                                t.Span(mesSeleccionado.ToUpper()).Bold().FontSize(10);
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1.5f).LineColor("#003366");
                    });

                    // Tabla de 31 Días
                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(20);  // No.
                                columns.RelativeColumn(1);   // Nombre del Alumno
                                for (int i = 1; i <= 31; i++) columns.ConstantColumn(18); // 31 Casillas
                            });

                            // Cabecera
                            table.Header(header =>
                            {
                                header.Cell().Background("#003366").Border(1).BorderColor(Colors.White).PaddingVertical(4).AlignCenter().Text("#").Bold().FontColor(Colors.White);
                                header.Cell().Background("#003366").Border(1).BorderColor(Colors.White).PaddingVertical(4).AlignCenter().Text("NOMBRE COMPLETO DEL ALUMNO").Bold().FontColor(Colors.White);
                                
                                for (int i = 1; i <= 31; i++)
                                {
                                    header.Cell().Background("#003366").Border(1).BorderColor(Colors.White).PaddingVertical(4).AlignCenter().Text(i.ToString()).Bold().FontColor(Colors.White);
                                }
                            });

                            // Filas de Alumnos
                            int index = 1;
                            foreach (var alumno in alumnos)
                            {
                                var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                string nombreMostrado = !string.IsNullOrWhiteSpace(alumno.Name) 
                                    ? $"{alumno.Name} {alumno.PaternalLastName} {alumno.MaternalLastName}".Trim() 
                                    : "ALUMNO DESCONOCIDO";

                                table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(index.ToString());
                                table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).PaddingLeft(4).AlignMiddle().Text(nombreMostrado.ToUpper()).Bold();
                                
                                // Generar las 31 casillas vacías
                                for (int i = 1; i <= 31; i++)
                                {
                                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text("");
                                }
                                index++;
                            }
                        });
                    });

                    // Pie de página (Firmas)
                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Width(200).LineHorizontal(1).LineColor(Colors.Black);
                                c.Item().PaddingTop(4).Text("NOMBRE Y FIRMA DEL DOCENTE").FontSize(9).Bold().FontColor(Colors.Grey.Darken3);
                            });

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Width(200).LineHorizontal(1).LineColor(Colors.Black);
                                c.Item().PaddingTop(4).Text("VO. BO. DIRECCIÓN GENERAL").FontSize(9).Bold().FontColor(Colors.Grey.Darken3);
                            });
                        });
                    });
                });
            }).GeneratePdf();
        }

        // ===================================================
        // 2. GENERAR BOLETA DE CALIFICACIONES
        // ===================================================
        public byte[] GenerarBoleta(Student alumno, List<FilaBoletaDto> calificaciones, string nombreCurso, string nombreEscuela = "EscuelaManager", string logoBase64 = "")
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().BorderBottom(2).BorderColor(Colors.Blue.Medium).PaddingBottom(10).Row(row =>
                    {
                        if (!string.IsNullOrEmpty(logoBase64))
                        {
                            var cleanLogo = logoBase64.Contains(',') ? logoBase64.Split(',')[1] : logoBase64;
                            row.ConstantItem(75).Image(Convert.FromBase64String(cleanLogo));
                            row.ConstantItem(15);
                        }

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(nombreEscuela).FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("BOLETA OFICIAL DE CALIFICACIONES").FontSize(12).Bold().FontColor(Colors.Grey.Darken2);
                        });
                    });

                    page.Content().PaddingTop(0.5f, Unit.Centimetre).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(t => { t.Span("Alumno: ").Bold(); t.Span($"{alumno.Name} {alumno.PaternalLastName} {alumno.MaternalLastName}".Trim()); });
                            row.RelativeItem().Text(t => { t.Span("Curso: ").Bold(); t.Span(nombreCurso); });
                        });

                        column.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text(t => { t.Span("ID Estudiante: ").Bold(); t.Span(alumno.Id?.Length >= 8 ? alumno.Id[..8].ToUpper() : alumno.Id?.ToUpper() ?? ""); });
                            row.RelativeItem().Text(t => { t.Span("Ciclo Escolar: ").Bold(); t.Span("2026-2027"); });
                        });

                        column.Item().PaddingTop(1.5f, Unit.Centimetre).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(3);
                                columnas.RelativeColumn(2);
                                columnas.RelativeColumn(1.5f);
                                columnas.ConstantColumn(80);
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(8).Text("Materia").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(8).Text("Profesor").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(8).Text("Periodo").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(8).Text("Nota").FontColor(Colors.White).Bold().AlignCenter();
                            });

                            double sumaNotas = 0;
                            foreach (var cal in calificaciones)
                            {
                                tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(cal.NombreMateria);
                                tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(cal.Profesor);
                                tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(cal.Periodo);

                                var celdaNota = tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignCenter();
                                if (cal.Nota < 6.0)
                                    celdaNota.Text(cal.Nota.ToString("0.0")).Bold().FontColor(Colors.Red.Medium);
                                else
                                    celdaNota.Text(cal.Nota.ToString("0.0")).Bold().FontColor(Colors.Green.Medium);

                                sumaNotas += cal.Nota;
                            }

                            if (calificaciones.Count > 0)
                            {
                                double promedio = sumaNotas / calificaciones.Count;
                                tabla.Cell().Padding(8).Text("");
                                tabla.Cell().Padding(8).Text("");
                                tabla.Cell().Padding(8).AlignRight().Text("PROMEDIO:").Bold();
                                var celdaPromedio = tabla.Cell().Background(Colors.Grey.Lighten3).Padding(8).AlignCenter();

                                if (promedio < 6.0)
                                    celdaPromedio.Text(promedio.ToString("0.0")).Bold().FontColor(Colors.Red.Medium);
                                else
                                    celdaPromedio.Text(promedio.ToString("0.0")).Bold().FontColor(Colors.Green.Medium);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Documento emitido de forma digital por Sistemas VENEGAS - ").FontSize(9).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }

        // ===================================================
        // 3. GENERAR CREDENCIAL DINÁMICA
        // ===================================================
        public byte[] GenerarCredencialEstudiante(Student alumno, string matricula, string curso, string vigencia, string director, string logoBase64, CredencialDiseno diseno)
        {
            _ = logoBase64; 

            return Document.Create(container =>
            {
                const float anchoCredencial = 85.6f;
                const float altoCredencial = 54f;

                container.Page(page =>
                {
                    page.Size(anchoCredencial, altoCredencial, Unit.Millimetre);
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));

                    page.Content().Layers(layers =>
                    {
                        layers.PrimaryLayer().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(diseno.PlantillaFrenteBase64))
                            {
                                var base64 = diseno.PlantillaFrenteBase64.Contains(',') ? diseno.PlantillaFrenteBase64.Split(',')[1] : diseno.PlantillaFrenteBase64;
                                e.Image(Convert.FromBase64String(base64)).FitArea();
                            }
                            else
                            {
                                e.Background(Colors.White).Width(anchoCredencial, Unit.Millimetre).Height(altoCredencial, Unit.Millimetre);
                            }
                        });

                        layers.Layer().PaddingLeft(diseno.FotoX, Unit.Millimetre).PaddingTop(diseno.FotoY, Unit.Millimetre)
                                      .Width(diseno.FotoW, Unit.Millimetre).Height(diseno.FotoH, Unit.Millimetre)
                                      .Element(e =>
                                      {
                                          if (!string.IsNullOrEmpty(alumno.PhotoBase64))
                                          {
                                              var base64 = alumno.PhotoBase64.Contains(',') ? alumno.PhotoBase64.Split(',')[1] : alumno.PhotoBase64;
                                              e.Image(Convert.FromBase64String(base64)).FitArea();
                                          }
                                          else
                                          {
                                              e.Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Text("FOTO").FontSize(8).FontColor(Colors.Grey.Medium);
                                          }
                                      });

                        layers.Layer().PaddingLeft(diseno.NombreX, Unit.Millimetre).PaddingTop(diseno.NombreY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("ALUMNO(A):").FontSize(diseno.NombreSize - 2).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text($"{alumno.Name} {alumno.PaternalLastName} {alumno.MaternalLastName}".ToUpper()).Bold().FontSize(diseno.NombreSize).FontColor("#003366");
                                      });

                        layers.Layer().PaddingLeft(diseno.MatriculaX, Unit.Millimetre).PaddingTop(diseno.MatriculaY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("MATRÍCULA:").FontSize(diseno.MatriculaSize - 3).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text(matricula).Bold().FontSize(diseno.MatriculaSize).FontColor(Colors.Red.Medium);
                                      });

                        layers.Layer().PaddingLeft(diseno.CursoX, Unit.Millimetre).PaddingTop(diseno.CursoY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("CURSO:").FontSize(diseno.CursoSize - 2).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text(curso).Bold().FontSize(diseno.CursoSize).FontColor(Colors.Black);
                                      });

                        layers.Layer().PaddingLeft(diseno.VigenciaX, Unit.Millimetre).PaddingTop(diseno.VigenciaY, Unit.Millimetre)
                                      .Text($"VIGENCIA: {vigencia}").Bold().FontSize(diseno.VigenciaSize).FontColor(Colors.Grey.Darken3);
                    });
                });

                container.Page(page =>
                {
                    page.Size(anchoCredencial, altoCredencial, Unit.Millimetre);
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));

                    page.Content().Layers(layers =>
                    {
                        layers.PrimaryLayer().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(diseno.PlantillaReversoBase64))
                            {
                                var base64 = diseno.PlantillaReversoBase64.Contains(',') ? diseno.PlantillaReversoBase64.Split(',')[1] : diseno.PlantillaReversoBase64;
                                e.Image(Convert.FromBase64String(base64)).FitArea();
                            }
                            else
                            {
                                e.Background(Colors.White).Width(anchoCredencial, Unit.Millimetre).Height(altoCredencial, Unit.Millimetre);
                            }
                        });

                        layers.Layer().PaddingLeft(diseno.FirmaLineaX, Unit.Millimetre).PaddingTop(diseno.FirmaLineaY, Unit.Millimetre)
                                      .Width(diseno.FirmaLineaW, Unit.Millimetre)
                                      .LineHorizontal(0.5f).LineColor(Colors.Black);

                        layers.Layer().PaddingLeft(diseno.DirectorX, Unit.Millimetre).PaddingTop(diseno.DirectorY, Unit.Millimetre)
                                      .Width(diseno.FirmaLineaW, Unit.Millimetre) 
                                      .Column(c =>
                                      {
                                          c.Item().AlignCenter().Text(director.ToUpper()).Bold().FontSize(diseno.DirectorSize).FontColor(Colors.Grey.Darken4);
                                          c.Item().AlignCenter().Text("DIRECTOR DEL PLANTEL").FontSize(diseno.DirectorSize - 1).FontColor(Colors.Grey.Darken2);
                                      });
                    });
                });
            }).GeneratePdf();
        }
    }

    public class FilaBoletaDto
    {
        public string NombreMateria { get; set; } = "";
        public string Profesor { get; set; } = "";
        public string Periodo { get; set; } = "";
        public double Nota { get; set; }
    }
}