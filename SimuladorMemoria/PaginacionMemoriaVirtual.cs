using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class PaginacionMemoriaVirtual: Memoria
    {
        private int tamanoPagina; // Tamaño de cada página
        private Queue<int> marcosLibres; // Cola de marcos libres
        private Dictionary<int, List<int>> tablaPaginas; // Tabla de páginas por proceso
        private Queue<int> fifoQueue; // Cola FIFO para las páginas en memoria física
        private HashSet<int> paginasEnMemoria; // Páginas actualmente en memoria física

        public PaginacionMemoriaVirtual(int tamanioTotal, int tamanoPagina) : base(tamanioTotal)
        {
            this.tamanoPagina = tamanoPagina;
            marcosLibres = new Queue<int>();
            tablaPaginas = new Dictionary<int, List<int>>();
            fifoQueue = new Queue<int>();
            paginasEnMemoria = new HashSet<int>();

            int numMarcos = tamanioTotal / tamanoPagina;
            for (int i = 0; i < numMarcos; i++)
            {
                marcosLibres.Enqueue(i);
            }
        }

        public int TamanoPagina => tamanoPagina; // Propiedad para obtener el tamaño de página
        public HashSet<int> PaginasEnMemoria => paginasEnMemoria; // Propiedad para las páginas físicas

        public override bool AsignarMemoria(Proceso proceso)
        {
            int paginasRequeridas = (int)Math.Ceiling((double)proceso.Tamanio / tamanoPagina);

            if (paginasRequeridas > marcosLibres.Count)
            {
                Console.WriteLine($"Error: No se puede asignar memoria al proceso. Espacio insuficiente para {paginasRequeridas} páginas.");
                return false;
            }

            // Agregar las páginas requeridas al proceso
            var paginasAsignadas = new List<int>();
            for (int i = 0; i < paginasRequeridas; i++)
            {
                int marco = marcosLibres.Dequeue(); // Tomar un marco libre
                paginasEnMemoria.Add(marco);
                paginasAsignadas.Add(marco);
            }

            tablaPaginas[proceso.Id] = paginasAsignadas;
            Procesos.Add(proceso);
            return true;
        }

        private bool ReemplazarPagina(int idProceso, int paginaIndex)
        {
            if (fifoQueue.Count == 0) return false;

            int marcoReemplazar = fifoQueue.Dequeue();
            paginasEnMemoria.Remove(marcoReemplazar);

            // Validar que el proceso tiene páginas asignadas
            if (!tablaPaginas.ContainsKey(idProceso) || paginaIndex >= tablaPaginas[idProceso].Count)
            {
                Console.WriteLine($"Error: El índice de la página ({paginaIndex}) no es válido para el proceso {idProceso}.");
                return false;
            }

            tablaPaginas[idProceso][paginaIndex] = marcoReemplazar;
            fifoQueue.Enqueue(marcoReemplazar);
            paginasEnMemoria.Add(marcoReemplazar);

            return true;
        }

        public override void LiberarMemoria(int idProceso)
        {
            if (tablaPaginas.ContainsKey(idProceso))
            {
                foreach (int marco in tablaPaginas[idProceso])
                {
                    paginasEnMemoria.Remove(marco);
                    fifoQueue = new Queue<int>(fifoQueue.Where(m => m != marco));
                    marcosLibres.Enqueue(marco);
                }
                tablaPaginas.Remove(idProceso);
                Procesos.RemoveAll(p => p.Id == idProceso);
            }
        }

        public List<int> ObtenerTablaPaginas(int idProceso)
        {
            return tablaPaginas.ContainsKey(idProceso) ? tablaPaginas[idProceso] : new List<int>();
        }

        public bool EstaEnMemoriaFisica(int pagina)
        {
            return paginasEnMemoria.Contains(pagina);
        }

    }
}
