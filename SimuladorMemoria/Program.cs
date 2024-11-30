using SimuladorMemoria;
using System;
using System.Diagnostics;
using System.Linq;

namespace SimuladorMemoriaConsola
{
    class Program
    {
        private static Memoria memoria;

        static void Main(string[] args)
        {
            ConfigurarMemoria();
            MenuPrincipal();
        }

        private static void ConfigurarMemoria()
        {
            Console.Clear();
            Console.WriteLine("========== Configuración de Memoria ==========");
            Console.Write("Ingrese el tamaño total de la memoria: ");
            int tamanioTotalMemoria = int.Parse(Console.ReadLine());

            Console.WriteLine("Seleccione el esquema de particionamiento:");
            Console.WriteLine("1. Fijo");
            Console.WriteLine("2. Dinámico");
            Console.WriteLine("3. Paginación");
            Console.WriteLine("4. Segmentación");
            int tipoParticionamiento = int.Parse(Console.ReadLine());

            switch (tipoParticionamiento)
            {
                case 1:
                    Console.Write("Ingrese el tamaño de cada partición: ");
                    int tamanioParticion = int.Parse(Console.ReadLine());

                    if (tamanioTotalMemoria % tamanioParticion != 0)
                    {
                        Console.WriteLine("Error: El tamaño total de la memoria debe ser múltiplo del tamaño de la partición.");
                        return;
                    }

                    memoria = new ParticionamientoFijo(tamanioTotalMemoria, tamanioParticion);
                    break;
                case 2:
                    memoria = new ParticionamientoDinamico(tamanioTotalMemoria);
                    break;
                case 3:
                    Console.Write("Ingrese el tamaño de la página: ");
                    int tamanoPagina = int.Parse(Console.ReadLine());
                    Console.Write("Ingrese el tamaño total de la memoria virtual: ");
                    int tamanoMemoriaVirtual = int.Parse(Console.ReadLine());

                    if (tamanoMemoriaVirtual % tamanoPagina != 0)
                    {
                        Console.WriteLine("Error: El tamaño de la memoria virtual debe ser múltiplo del tamaño de la página.");
                        return;
                    }

                    Console.WriteLine("Seleccione el algoritmo de reemplazo de páginas:");
                    Console.WriteLine("1. FIFO");
                    Console.WriteLine("2. LRU");
                    Console.WriteLine("3. Reloj");
                    int algoritmoSeleccionado = int.Parse(Console.ReadLine());
                    string algoritmo = algoritmoSeleccionado switch
                    {
                        1 => "FIFO",
                        2 => "LRU",
                        3 => "Reloj",
                        _ => throw new ArgumentException("Opción inválida.")
                    };

                    memoria = new PaginacionMemoriaVirtual(tamanioTotalMemoria, tamanoPagina, tamanoMemoriaVirtual, algoritmo);
                    break;
                case 4:
                    memoria = new Segmentacion(tamanioTotalMemoria);
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    return;
            }

            Console.WriteLine("Memoria configurada exitosamente.");
        }

        private static void MenuPrincipal()
        {
            int opcion;
            do
            {
                Console.Clear();
                Console.WriteLine("========== Simulador de Memoria ==========");
                Console.WriteLine("1. Configurar Memoria (Fijo, Dinámico, Paginación, Segmentación)");
                Console.WriteLine("2. Agregar Proceso");
                Console.WriteLine("3. Liberar Proceso");
                Console.WriteLine("4. Ver Estado de la Memoria");
                Console.WriteLine("5. Salir");
                Console.WriteLine("===========================================");
                Console.Write("Seleccione una opción: ");
                opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        ConfigurarMemoria();
                        break;
                    case 2:
                        AgregarProceso();
                        break;
                    case 3:
                        LiberarProceso();
                        break;
                    case 4:
                        VerEstadoMemoriaVisual();
                        break;
                    case 5:
                        Console.WriteLine("Saliendo del simulador...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                if (opcion != 5)
                {
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
                    Console.ReadKey();
                }

            } while (opcion != 5);
        }

        private static void AgregarProceso()
        {
            Console.Clear();
            Console.WriteLine("========== Agregar Proceso ==========");
            Console.Write("Ingrese el ID del proceso: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Ingrese el tamanio del proceso: ");
            int tamanio = int.Parse(Console.ReadLine());

            if (memoria.Procesos.Any(p => p.Id == id))
            {
                Console.WriteLine("Error: Ya existe un proceso con el mismo ID.");
                return;
            }

            Proceso proceso = new Proceso(id, tamanio);
            if (memoria.AsignarMemoria(proceso))
            {
                Console.WriteLine("Proceso agregado exitosamente.");
            }
            else
            {
                Console.WriteLine("Error: No se pudo asignar memoria al proceso. Espacio insuficiente o proceso muy grande.");
            }
        }

        private static void LiberarProceso()
        {
            Console.Clear();
            Console.WriteLine("========== Liberar Proceso ==========");
            Console.Write("Ingrese el ID del proceso a liberar: ");
            int id = int.Parse(Console.ReadLine());

            if (!memoria.Procesos.Any(p => p.Id == id))
            {
                Console.WriteLine("Error: El proceso con el ID especificado no existe en la memoria.");
                return;
            }

            memoria.LiberarMemoria(id);
            Console.WriteLine("Proceso liberado exitosamente.");
        }

        private static void VerEstadoMemoriaVisual()
        {
            Console.Clear();
            Console.WriteLine("========== Estado de la Memoria ==========");

            // Identificar el tipo de memoria
            if (memoria is ParticionamientoFijo || memoria is ParticionamientoDinamico)
            {
                MostrarEstadoParticionamiento(); // Método para particionamiento fijo/dinámico
            }
            else if (memoria is PaginacionMemoriaVirtual paginacion)
            {
                paginacion.MostrarEstadoMemoria();
            }
            else if (memoria is Segmentacion segmentacion)
            {
                MostrarEstadoSegmentacion(segmentacion); // Método para segmentación
            }
            else
            {
                Console.WriteLine("Error: Tipo de memoria desconocido.");
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }

        private static void MostrarEstadoParticionamiento()
        {
            Console.WriteLine($"Memoria Física Total: {memoria.TamanioTotal}");
            Console.WriteLine($"Memoria Física Usada: {memoria.Procesos.Sum(p => p.Tamanio)}");
            Console.WriteLine($"Memoria Física Libre: {Math.Max(memoria.TamanioTotal - memoria.Procesos.Sum(p => p.Tamanio), 0)}");
            Console.WriteLine("\nProcesos en memoria:");

            if (memoria.Procesos.Count == 0)
            {
                Console.WriteLine("No hay procesos en memoria.");
            }
            else
            {
                foreach (var proceso in memoria.Procesos)
                {
                    Console.WriteLine($"Proceso {proceso.Id} - Tamaño: {proceso.Tamanio}");
                }
            }

            // Representación visual
            Console.Write("\nMemoria Física: [");
            int totalSize = memoria.TamanioTotal;
            int blockCount = 50;
            int usedBlocks = (int)((double)memoria.Procesos.Sum(p => p.Tamanio) / totalSize * blockCount);
            Console.Write(new string('█', usedBlocks));
            Console.Write(new string('░', blockCount - usedBlocks));
            Console.WriteLine("]");
        }
        private static void MostrarEstadoSegmentacion(Segmentacion segmentacion)
        {
            Console.WriteLine($"Tamaño Total: {segmentacion.TamanioTotal}");

            // Mostrar segmentos ocupados
            if (segmentacion.Procesos.Count == 0)
            {
                Console.WriteLine("No hay segmentos asignados.");
            }
            else
            {
                Console.WriteLine("Segmentos ocupados:");
                foreach (var segmento in segmentacion.ObtenerSegmentos())
                {
                    Console.WriteLine($"- Proceso {segmento.idProceso}: Inicio {segmento.inicio}, Tamaño {segmento.tamanio}");
                }
            }

            // Mostrar espacios libres
            var espaciosLibres = segmentacion.ObtenerEspaciosLibres();
            if (espaciosLibres.Count > 0)
            {
                Console.WriteLine("\nFragmentación externa (segmentos libres):");
                foreach (var espacio in espaciosLibres)
                {
                    Console.WriteLine($"Espacio libre desde posición {espacio.inicio} con tamaño {espacio.tamanio}");
                }
            }
            else
            {
                Console.WriteLine("\nNo hay fragmentación externa.");
            }

            // Representación visual
            Console.Write("\nMemoria Física: [");
            int posicionActual = 0;

            foreach (var segmento in segmentacion.ObtenerSegmentos().OrderBy(s => s.inicio))
            {
                if (posicionActual < segmento.inicio)
                {
                    int hueco = segmento.inicio - posicionActual;
                    Console.Write(new string('░', Math.Min(hueco / 10, 50)));
                }

                Console.Write(new string('█', Math.Min(segmento.tamanio / 10, 50)));
                posicionActual = segmento.inicio + segmento.tamanio;
            }

            if (posicionActual < segmentacion.TamanioTotal)
            {
                int huecoFinal = segmentacion.TamanioTotal - posicionActual;
                Console.Write(new string('░', Math.Min(huecoFinal / 10, 50)));
            }
            Console.WriteLine("]");
        }

    }
}
