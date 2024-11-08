using SimuladorMemoria;
using System;
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

            Console.Write("Seleccione el esquema de particionamiento (1 = Fijo, 2 = Dinamico): ");
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
                        Console.WriteLine("Opcion no valida. Intente nuevamente.");
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

            // Forma visual de la memoria
            int totalSize = memoria.TamanioTotal;
            int usedSize = memoria.Procesos.Sum(p => p.Tamanio);
            int freeSize = totalSize - usedSize;

            Console.WriteLine("Memoria: ");
            Console.Write("[");

            int blockCount = 50; // Representa el numero de bloques en la memoria
            int usedBlocks = (int)((double)usedSize / totalSize * blockCount);
            int freeBlocks = blockCount - usedBlocks;

            Console.Write(new string('█', usedBlocks)); // Bloques que estan ocupados
            Console.Write(new string('░', freeBlocks)); // Bloques que estan libres libres

            Console.WriteLine("]");

            Console.WriteLine("\nProcesos en memoria:");
            if (memoria.Procesos.Count == 0)
            {
                Console.WriteLine("No hay procesos en memoria.");
            }
            else
            {
                foreach (var proceso in memoria.Procesos)
                {
                    Console.WriteLine($"Proceso {proceso.Id} - Tamanio: {proceso.Tamanio}");
                }
            }
        }
    }
}
