using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class PaginacionMemoriaVirtual: Memoria
    {
        private int tamanoPagina;
        private Queue<int> marcosLibres;
        private Queue<int> marcosVirtuales;
        private Dictionary<int, List<int>> tablaPaginas;
        private Queue<int> fifoQueue;
        private Dictionary<int, int> ubicacionPaginas;
        private string algoritmoReemplazo;



        public PaginacionMemoriaVirtual(int tamanioTotal, int tamanoPagina, int tamanioVirtual, string algoritmo) : base(tamanioTotal)
        {
            this.tamanoPagina = tamanoPagina;
            algoritmoReemplazo = algoritmo;

            marcosLibres = new Queue<int>();
            marcosVirtuales = new Queue<int>();
            tablaPaginas = new Dictionary<int, List<int>>();
            fifoQueue = new Queue<int>();
            ubicacionPaginas = new Dictionary<int, int>();
            paginasEnMemoria = new List<int>();

            int numMarcosFisicos = tamanioTotal / tamanoPagina;
            for (int i = 0; i < numMarcosFisicos; i++)
            {
                marcosLibres.Enqueue(i);
            }

            int numMarcosVirtuales = tamanioVirtual / tamanoPagina;
            for (int i = numMarcosFisicos; i < numMarcosFisicos + numMarcosVirtuales; i++)
            {
                marcosVirtuales.Enqueue(i);
            }
        }

        public override bool AsignarMemoria(Proceso proceso)
        {
            int paginasNecesarias = (int)Math.Ceiling((double)proceso.Tamanio / tamanoPagina);

            int totalMarcosDisponibles = marcosLibres.Count + marcosVirtuales.Count;
            if (paginasNecesarias > totalMarcosDisponibles)
            {
                Console.WriteLine($"Error: No hay suficiente espacio en memoria física y virtual para el proceso {proceso.Id}.");
                return false;
            }

            int memoriaUsadaTotal = Procesos.Sum(p => p.Tamanio) + proceso.Tamanio;
            int memoriaDisponibleTotal = TamanioTotal + marcosVirtuales.Count * tamanoPagina;

            if (memoriaUsadaTotal > memoriaDisponibleTotal)
            {
                Console.WriteLine($"Error: El proceso {proceso.Id} excede la capacidad total de la memoria.");
                return false;
            }

            if (!tablaPaginas.ContainsKey(proceso.Id))
            {
                tablaPaginas[proceso.Id] = new List<int>();
            }

            // Asigna paginas al proceso
            for (int i = 0; i < paginasNecesarias; i++)
            {
                if (marcosLibres.Count > 0)
                {
                    // Asignar un marco fisico libre
                    int marco = marcosLibres.Dequeue();
                    tablaPaginas[proceso.Id].Add(marco);
                    fifoQueue.Enqueue(marco);
                    paginasEnMemoria.Add(marco);
                }
                else
                {
                    // Manejo de fallos de pagina si no hay marcos libres
                    if (!ManejarFalloDePagina(proceso.Id, i))
                    {
                        Console.WriteLine($"Error: No se pudo asignar la página {i} del proceso {proceso.Id}.");
                        return false;
                    }
                }
            }

            Procesos.Add(proceso);
            return true;
        }

        private bool ManejarFalloDePagina(int idProceso, int paginaIndex)
        {
            if (fifoQueue.Count == 0)
            {
                Console.WriteLine("Error: No hay marcos en memoria física para reemplazar.");
                return false;
            }

            int marcoReemplazar = fifoQueue.Dequeue();

            paginasEnMemoria.Remove(marcoReemplazar);
            marcosVirtuales.Enqueue(marcoReemplazar);

            if (ubicacionPaginas.TryGetValue(marcoReemplazar, out int procesoAnterior))
            {
                if (tablaPaginas.ContainsKey(procesoAnterior))
                {
                    tablaPaginas[procesoAnterior].Remove(marcoReemplazar);
                }
            }

            // Asignar el marco al nuevo proceso
            fifoQueue.Enqueue(marcoReemplazar);
            ubicacionPaginas[marcoReemplazar] = idProceso;
            tablaPaginas[idProceso].Add(marcoReemplazar);

            return true;
        }

        public override void LiberarMemoria(int idProceso)
        {
            if (!tablaPaginas.ContainsKey(idProceso))
            {
                Console.WriteLine($"Error: El proceso {idProceso} no existe.");
                return;
            }

            foreach (int marco in tablaPaginas[idProceso])
            {
                if (paginasEnMemoria.Contains(marco))
                {
                    paginasEnMemoria.Remove(marco);
                    marcosLibres.Enqueue(marco);
                }
                else
                {
                    marcosVirtuales.Enqueue(marco);
                }
                ubicacionPaginas.Remove(marco);
            }

            tablaPaginas.Remove(idProceso);
            Procesos.RemoveAll(p => p.Id == idProceso);
        }

        public void MostrarEstadoMemoria()
        {
            Console.WriteLine($"Memoria Física Total: {TamanioTotal}");
            Console.WriteLine($"Memoria Física Usada: {paginasEnMemoria.Count * tamanoPagina}");
            Console.WriteLine($"Memoria Física Libre: {marcosLibres.Count * tamanoPagina}");
            int totalVirtualSize = marcosVirtuales.Count * tamanoPagina + paginasEnMemoria.Count * tamanoPagina;
            int virtualUsedMemory = tablaPaginas.Values.Sum(lista => lista.Count(pagina => !paginasEnMemoria.Contains(pagina))) * tamanoPagina;
            Console.WriteLine($"Memoria Virtual Libre: {totalVirtualSize - virtualUsedMemory}");

            Console.WriteLine("\nProcesos en memoria:");
            if (Procesos.Count == 0)
            {
                Console.WriteLine("No hay procesos en memoria.");
            }
            else
            {
                foreach (var proceso in Procesos)
                {
                    Console.WriteLine($"Proceso {proceso.Id} - Tamaño: {proceso.Tamanio}");
                    var paginas = ObtenerTablaPaginas(proceso.Id);
                    for (int i = 0; i < paginas.Count; i++)
                    {
                        string ubicacion = paginasEnMemoria.Contains(paginas[i]) ? "Memoria Física" : "Memoria Virtual";
                        Console.WriteLine($"  Página {i} - Marco {paginas[i]} ({ubicacion})");
                    }
                }
            }

            // === Agrega puntos de depuración ===
            Console.WriteLine("\nDepuración:");
            Console.WriteLine($"Páginas en memoria física: {string.Join(", ", paginasEnMemoria)}");
            Console.WriteLine($"Marcos libres: {string.Join(", ", marcosLibres)}");
            Console.WriteLine($"Marcos virtuales: {string.Join(", ", marcosVirtuales)}");

            

            Console.Write("\nMemoria Física: [");
            int blockCount = 50;
            int usedBlocks = (int)((double)paginasEnMemoria.Count * tamanoPagina / TamanioTotal * blockCount);
            Console.Write(new string('█', usedBlocks));
            Console.Write(new string('░', blockCount - usedBlocks));
            Console.WriteLine("]");

            Console.Write("\nMemoria Virtual: [");
            int virtualUsedBlocks = (int)((double)virtualUsedMemory / totalVirtualSize * blockCount);
            Console.Write(new string('█', virtualUsedBlocks));
            Console.Write(new string('░', blockCount - virtualUsedBlocks));
            Console.WriteLine("]");
        }


        public List<int> ObtenerTablaPaginas(int idProceso)
        {
            return tablaPaginas.ContainsKey(idProceso) ? tablaPaginas[idProceso] : new List<int>();
        }
    }
}
