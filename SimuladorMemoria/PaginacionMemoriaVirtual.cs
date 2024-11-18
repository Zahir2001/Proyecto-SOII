using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class PaginacionMemoriaVirtual: Memoria
    {
        private int tamanoPagina;
        private Queue<int> marcosLibres;
        private Dictionary<int, List<int>> tablaPaginas;
        private Queue<int> fifoQueue; // Cola FIFO para las páginas en memoria física
        private HashSet<int> paginasEnMemoria; // Páginas actualmente en memoria

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

        public override bool AsignarMemoria(Proceso proceso)
        {
            int numPaginas = (int)Math.Ceiling((double)proceso.Tamanio / tamanoPagina);
            tablaPaginas[proceso.Id] = new List<int>();

            for (int i = 0; i < numPaginas; i++)
            {
                if (marcosLibres.Count > 0)
                {
                    int marco = marcosLibres.Dequeue();
                    tablaPaginas[proceso.Id].Add(marco);
                    fifoQueue.Enqueue(marco);
                    paginasEnMemoria.Add(marco);
                }
                else
                {
                    if (!ReemplazarPagina(proceso.Id, i)) return false;
                }
            }

            Procesos.Add(proceso);
            return true;
        }

        private bool ReemplazarPagina(int idProceso, int paginaIndex)
        {
            if (fifoQueue.Count == 0) return false;

            int marcoReemplazar = fifoQueue.Dequeue();
            paginasEnMemoria.Remove(marcoReemplazar);

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
