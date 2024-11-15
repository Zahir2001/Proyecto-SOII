using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class Paginacion: Memoria
    {
        private int tamanoPagina;
        private Queue<int> marcosLibres;
        private Dictionary<int, List<int>> tablaPaginas;

        public Paginacion(int tamanioTotal, int tamanoPagina) : base(tamanioTotal)
        {
            this.tamanoPagina = tamanoPagina;
            marcosLibres = new Queue<int>();
            tablaPaginas = new Dictionary<int, List<int>>();

            int numMarcos = tamanioTotal / tamanoPagina;
            for (int i = 0; i < numMarcos; i++)
            {
                marcosLibres.Enqueue(i);
            }
        }

        public override bool AsignarMemoria(Proceso proceso)
        {
            int numPaginas = (int)Math.Ceiling((double)proceso.Tamanio / tamanoPagina);
            if (marcosLibres.Count < numPaginas)
            {
                return false; // Espacio insuficiente en marcos
            }

            tablaPaginas[proceso.Id] = new List<int>();

            for (int i = 0; i < numPaginas; i++)
            {
                int marco = marcosLibres.Dequeue();
                tablaPaginas[proceso.Id].Add(marco);
            }

            Procesos.Add(proceso);
            return true;
        }

        public override void LiberarMemoria(int idProceso)
        {
            if (tablaPaginas.ContainsKey(idProceso))
            {
                foreach (int marco in tablaPaginas[idProceso])
                {
                    marcosLibres.Enqueue(marco);
                }
                tablaPaginas.Remove(idProceso);
                Procesos.RemoveAll(p => p.Id == idProceso);
            }
        }
    }
}
