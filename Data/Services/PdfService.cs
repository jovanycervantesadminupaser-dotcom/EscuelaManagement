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
        // 1. GENERAR LISTA DE ASISTENCIA
        // ===================================================
        public byte[] GenerarListaAsistencia(List<Student> alumnos, string nombreCurso, string nombreEscuela = "EscuelaManager", string logoBase64 = "")
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Encabezado
                    page.Header().Row(row =>
                    {
                        if (!string.IsNullOrEmpty(logoBase64))
                        {
                            var base64Data = logoBase64.Contains(",") ? logoBase64.Substring(logoBase64.IndexOf(",") + 1) : logoBase64;
                            var imageBytes = Convert.FromBase64String(base64Data);
                            row.ConstantItem(60).Image(imageBytes);
                            row.ConstantItem(15); // Espaciador
                        }

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(nombreEscuela).FontSize(22).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Lista de Asistencia - Curso: {nombreCurso}").FontSize(14).Medium();
                            col.Item().Text($"Fecha de Impresión: {DateTime.Now.ToString("dd/MM/yyyy")}").FontSize(9).Italic();
                        });
                    });

                    // Contenido (Tabla de alumnos con columnas de asistencia)
                    page.Content().PaddingTop(1, Unit.Centimetre).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.ConstantColumn(30);  // No.
                            columnas.RelativeColumn(3);   // Nombre del Alumno
                            columnas.RelativeColumn(1);   // Clase 1
                            columnas.RelativeColumn(1);   // Clase 2
                            columnas.RelativeColumn(1);   // Clase 3
                            columnas.RelativeColumn(1);   // Clase 4
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("#").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Nombre del Alumno").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("Firma").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("").Bold();
                            header.Cell().Background(Colors.Blue.Lighten4).Padding(5).Text("").Bold();
                        });

                        int index = 1;
                        foreach (var alumno in alumnos)
                        {
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(index.ToString());
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(alumno.Name ?? "Sin Nombre");
                            
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                            index++;
                        }
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // ===================================================
        // 2. GENERAR BOLETA DE CALIFICACIONES
        // ===================================================
        public byte[] GenerarBoleta(Student alumno, List<FilaBoletaDto> calificaciones, string nombreCurso, string nombreEscuela = "EscuelaManager", string logoBase64 = "")
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Encabezado Institucional
                    page.Header().BorderBottom(2).BorderColor(Colors.Blue.Medium).PaddingBottom(10).Row(row =>
                    {
                        if (!string.IsNullOrEmpty(logoBase64))
                        {
                            var base64Data = logoBase64.Contains(",") ? logoBase64.Substring(logoBase64.IndexOf(",") + 1) : logoBase64;
                            var imageBytes = Convert.FromBase64String(base64Data);
                            row.ConstantItem(75).Image(imageBytes);
                            row.ConstantItem(15); // Espaciador
                        }

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(nombreEscuela).FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("BOLETA OFICIAL DE CALIFICACIONES").FontSize(12).Bold().FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // Información del Estudiante
                    page.Content().PaddingTop(0.5f, Unit.Centimetre).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(t => { t.Span("Alumno: ").Bold(); t.Span(alumno.Name ?? ""); });
                            row.RelativeItem().Text(t => { t.Span("Curso: ").Bold(); t.Span(nombreCurso); });
                        });
                        
                        column.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text(t => { t.Span("ID Estudiante: ").Bold(); t.Span(alumno.Id?.Substring(0, 8).ToUpper() ?? ""); });
                            row.RelativeItem().Text(t => { t.Span("Ciclo Escolar: ").Bold(); t.Span("2026-2027"); });
                        });

                        // Tabla de Calificaciones
                        column.Item().PaddingTop(1.5f, Unit.Centimetre).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(3); // Materia
                                columnas.RelativeColumn(2); // Profesor
                                columnas.RelativeColumn(1.5f); // Periodo
                                columnas.ConstantColumn(80); // Calificación
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

                            // Fila de Promedio Final
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
            });

            return documento.GeneratePdf();
        }

        // ===================================================
        // 3. GENERAR CREDENCIAL DINÁMICA (CAPAS ABSOLUTAS CR80)
        // ===================================================
        public byte[] GenerarCredencialEstudiante(Student alumno, string matricula, string curso, string vigencia, string director, string logoBase64, CredencialDiseno diseno)
        {
            return Document.Create(container =>
            {
                // Medidas estándar de una tarjeta de PVC CR80
                var anchoCredencial = 85.6f;
                var altoCredencial = 54f;

                // --- PÁGINA 1: FRENTE ---
                container.Page(page =>
                {
                    page.Size(anchoCredencial, altoCredencial, Unit.Millimetre);
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));

                    page.Content().Layers(layers =>
                    {
                        // Capa 1: Imagen de Plantilla de Fondo (Frente)
                        layers.Layer().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(diseno.PlantillaFrenteBase64))
                            {
                                var base64 = diseno.PlantillaFrenteBase64.Contains(",") ? diseno.PlantillaFrenteBase64.Substring(diseno.PlantillaFrenteBase64.IndexOf(",") + 1) : diseno.PlantillaFrenteBase64;
                                e.Image(Convert.FromBase64String(base64)).FitArea();
                            }
                            else
                            {
                                // Fondo blanco de respaldo si no hay plantilla
                                e.Background(Colors.White).Width(anchoCredencial, Unit.Millimetre).Height(altoCredencial, Unit.Millimetre);
                            }
                        });

                        // Capa 2: Fotografía del Alumno
                        layers.Layer().PaddingLeft(diseno.FotoX, Unit.Millimetre).PaddingTop(diseno.FotoY, Unit.Millimetre)
                                      .Width(diseno.FotoW, Unit.Millimetre).Height(diseno.FotoH, Unit.Millimetre)
                                      .Element(e =>
                                      {
                                          if (!string.IsNullOrEmpty(alumno.PhotoBase64))
                                          {
                                              var base64 = alumno.PhotoBase64.Contains(",") ? alumno.PhotoBase64.Substring(alumno.PhotoBase64.IndexOf(",") + 1) : alumno.PhotoBase64;
                                              e.Image(Convert.FromBase64String(base64)).FitArea();
                                          }
                                          else
                                          {
                                              e.Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Text("FOTO").FontSize(8).FontColor(Colors.Grey.Medium);
                                          }
                                      });

                        // Capa 3: Nombre del Alumno
                        layers.Layer().PaddingLeft(diseno.NombreX, Unit.Millimetre).PaddingTop(diseno.NombreY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("ALUMNO(A):").FontSize(diseno.NombreSize - 2).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text($"{alumno.Name} {alumno.PaternalLastName} {alumno.MaternalLastName}".ToUpper()).Bold().FontSize(diseno.NombreSize).FontColor("#003366");
                                      });

                        // Capa 4: Matrícula
                        layers.Layer().PaddingLeft(diseno.MatriculaX, Unit.Millimetre).PaddingTop(diseno.MatriculaY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("MATRÍCULA:").FontSize(diseno.MatriculaSize - 3).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text(matricula).Bold().FontSize(diseno.MatriculaSize).FontColor(Colors.Red.Medium);
                                      });

                        // Capa 5: Curso
                        layers.Layer().PaddingLeft(diseno.CursoX, Unit.Millimetre).PaddingTop(diseno.CursoY, Unit.Millimetre)
                                      .Column(c =>
                                      {
                                          c.Item().Text("CURSO:").FontSize(diseno.CursoSize - 2).FontColor(Colors.Grey.Darken2);
                                          c.Item().Text(curso).Bold().FontSize(diseno.CursoSize).FontColor(Colors.Black);
                                      });

                        // Capa 6: Vigencia
                        layers.Layer().PaddingLeft(diseno.VigenciaX, Unit.Millimetre).PaddingTop(diseno.VigenciaY, Unit.Millimetre)
                                      .Text($"VIGENCIA: {vigencia}").Bold().FontSize(diseno.VigenciaSize).FontColor(Colors.Grey.Darken3);
                    });
                });

                // --- PÁGINA 2: REVERSO ---
                container.Page(page =>
                {
                    page.Size(anchoCredencial, altoCredencial, Unit.Millimetre);
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));

                    page.Content().Layers(layers =>
                    {
                        // Capa 1: Imagen de Plantilla de Fondo (Reverso)
                        layers.Layer().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(diseno.PlantillaReversoBase64))
                            {
                                var base64 = diseno.PlantillaReversoBase64.Contains(",") ? diseno.PlantillaReversoBase64.Substring(diseno.PlantillaReversoBase64.IndexOf(",") + 1) : diseno.PlantillaReversoBase64;
                                e.Image(Convert.FromBase64String(base64)).FitArea();
                            }
                            else
                            {
                                e.Background(Colors.White).Width(anchoCredencial, Unit.Millimetre).Height(altoCredencial, Unit.Millimetre);
                            }
                        });

                        // Capa 2: Línea de Firma
                        layers.Layer().PaddingLeft(diseno.FirmaLineaX, Unit.Millimetre).PaddingTop(diseno.FirmaLineaY, Unit.Millimetre)
                                      .Width(diseno.FirmaLineaW, Unit.Millimetre)
                                      .LineHorizontal(0.5f).LineColor(Colors.Black);

                        // Capa 3: Datos de la Dirección
                        layers.Layer().PaddingLeft(diseno.DirectorX, Unit.Millimetre).PaddingTop(diseno.DirectorY, Unit.Millimetre)
                                      .Width(diseno.FirmaLineaW, Unit.Millimetre) // Mismo ancho de la línea para centrado seguro
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

    // ===================================================
    // DTO DE TRANSFERENCIA
    // ===================================================
    public class FilaBoletaDto
    {
        public string NombreMateria { get; set; } = "";
        public string Profesor { get; set; } = "";
        public string Periodo { get; set; } = "";
        public double Nota { get; set; }
    }
}