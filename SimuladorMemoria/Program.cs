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
            Console.WriteLine("========== Configuracion de Memoria ==========");
            Console.Write("Ingrese el tamanio total de la memoria: ");
            int tamanioTotalMemoria = int.Parse(Console.ReadLine());

            Console.WriteLine("Seleccione el esquema de particionamiento:");
            Console.WriteLine("1. Particionamiento Fijo");
            Console.WriteLine("2. Particionamiento Dinamico");
            Console.WriteLine("3. Paginacion");
            Console.WriteLine("4. Segmentacion");
            Console.Write("Ingrese su opcion: ");
            int tipoParticionamiento = int.Parse(Console.ReadLine());

            if (tipoParticionamiento == 1)
            {
                Console.Write("Ingrese el tamanio de cada particion: ");
                int tamanioParticion = int.Parse(Console.ReadLine());

                if (tamanioTotalMemoria % tamanioParticion != 0)
                {
                    Console.WriteLine("Error: El tamanio total de la memoria debe ser multiplo del tamanio de la particion.");
                    return;
                }

                memoria = new ParticionamientoFijo(tamanioTotalMemoria, tamanioParticion);
            }
            else if (tipoParticionamiento == 2)
            {
                memoria = new ParticionamientoDinamico(tamanioTotalMemoria);
            }
            else if (tipoParticionamiento == 3)
            {
                Console.Write("Ingrese el tamanio de cada pagina: ");
                int tamanioPagina = int.Parse(Console.ReadLine());
                memoria = new PaginacionMemoriaVirtual(tamanioTotalMemoria, tamanioPagina);
            }
            else if (tipoParticionamiento == 4)
            {
                memoria = new Segmentacion(tamanioTotalMemoria);
            }
            else
            {
                Console.WriteLine("Opcion no valida.");
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
                Console.WriteLine("1. Agregar Proceso");
                Console.WriteLine("2. Liberar Proceso");
                Console.WriteLine("3. Ver Estado de la Memoria");
                Console.WriteLine("4. Salir");
                Console.WriteLine("===========================================");
                Console.Write("Seleccione una opcion: ");
                opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        AgregarProceso();
                        break;
                    case 2:
                        LiberarProceso();
                        break;
                    case 3:
                        VerEstadoMemoriaVisual();
                        break;
                    case 4:
                        Console.WriteLine("Saliendo del simulador...");
                        break;
                    default:
                        Console.WriteLine("Opcion no valida. Intenta nuevamente.");
                        break;
                }

                if (opcion != 4)
                {
                    Console.WriteLine("\nPresione cualquier tecla para volver al menu...");
                    Console.ReadKey();
                }

            } while (opcion != 4);
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

            int totalSize = memoria.TamanioTotal;
            int usedSize = 0;

            // Si la memoria es de segmentación, mostrar segmentos asignados
            if (memoria is Segmentacion segmentacion)
            {
                Console.WriteLine("Segmentos asignados:");
                foreach (var segmento in segmentacion.ObtenerSegmentosPorProceso())
                {
                    Console.WriteLine($"Proceso {segmento.idProceso}: Inicio {segmento.inicio}, Tamaño {segmento.tamanio}");
                }

                // Mostrar espacios libres
                Console.WriteLine("\nEspacios libres:");
                int espacioLibre = 0;
                foreach (var segmento in segmentacion.ObtenerSegmentosPorProceso().OrderBy(s => s.inicio))
                {
                    if (segmento.inicio > espacioLibre)
                    {
                        Console.WriteLine($"Inicio: {espacioLibre}, Tamaño libre: {segmento.inicio - espacioLibre}");
                    }
                    espacioLibre = segmento.inicio + segmento.tamanio;
                }

                if (totalSize > espacioLibre)
                {
                    Console.WriteLine($"Inicio: {espacioLibre}, Tamaño libre: {totalSize - espacioLibre}");
                }

                usedSize = segmentacion.ObtenerSegmentosPorProceso().Sum(s => s.tamanio);
            }
            else if (memoria is PaginacionMemoriaVirtual paginacion) // Manejo de paginación
            {
                usedSize = paginacion.PaginasEnMemoria.Count * paginacion.TamanoPagina;

                Console.WriteLine("Procesos en memoria:");
                if (memoria.Procesos.Count == 0)
                {
                    Console.WriteLine("No hay procesos en memoria.");
                }
                else
                {
                    foreach (var proceso in memoria.Procesos)
                    {
                        Console.WriteLine($"Proceso {proceso.Id} - Tamaño: {proceso.Tamanio}");
                        var paginas = paginacion.ObtenerTablaPaginas(proceso.Id);
                        if (paginas == null || paginas.Count == 0)
                        {
                            Console.WriteLine($" Proceso {proceso.Id} - No tiene páginas asignadas.");
                            continue;
                        }

                        for (int i = 0; i < paginas.Count; i++)
                        {
                            string estado = paginacion.EstaEnMemoriaFisica(paginas[i]) ? "En memoria física" : "En memoria virtual";
                            Console.WriteLine($" Página {i} - {estado}");
                        }
                    }
                }
            }
            else // Caso general (fijo/dinámico)
            {
                usedSize = memoria.Procesos.Sum(p => p.Tamanio);
                Console.WriteLine("Procesos en memoria:");
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
            }

            int freeSize = totalSize - usedSize;

            // Validar que usedSize no exceda totalSize
            if (usedSize > totalSize)
            {
                Console.WriteLine("Error: La memoria usada excede la memoria total. Verifique el cálculo de asignaciones.");
                return;
            }

            // Visualizar barra de memoria física
            Console.WriteLine("\nMemoria Física:");
            Console.Write("[");
            int blockCount = 50; // Número de bloques visuales
            int[] memoriaVisual = new int[blockCount]; // 0 = libre, 1 = ocupado

            if (memoria is Segmentacion segmentacionMemoria)
            {
                foreach (var segmento in segmentacionMemoria.ObtenerSegmentosPorProceso())
                {
                    int inicioBloque = segmento.inicio * blockCount / totalSize;
                    int finBloque = (segmento.inicio + segmento.tamanio) * blockCount / totalSize;

                    for (int i = inicioBloque; i < finBloque && i < blockCount; i++)
                    {
                        memoriaVisual[i] = 1; // Marcar como ocupado
                    }
                }
            }
            else if (memoria is PaginacionMemoriaVirtual paginacionMemoria)
            {
                foreach (var marco in paginacionMemoria.PaginasEnMemoria)
                {
                    int bloque = marco * blockCount / totalSize;
                    if (bloque < blockCount)
                    {
                        memoriaVisual[bloque] = 1; // Marcar como ocupado
                    }
                }
            }
            else
            {
                int ocupados = (int)((double)usedSize / totalSize * blockCount);
                for (int i = 0; i < ocupados; i++)
                {
                    memoriaVisual[i] = 1; // Marcar bloques ocupados
                }
            }

            foreach (var bloque in memoriaVisual)
            {
                Console.Write(bloque == 1 ? '█' : '░');
            }

            Console.WriteLine("]");

            Console.WriteLine($"\nFragmentación externa: {freeSize} libres en memoria física.");
        }
    }
}
