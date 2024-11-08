using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public abstract class Memoria
    {
        protected int tamanioTotal;
        public List<Proceso> Procesos { get; protected set; }

        public Memoria(int tamanio)
        {
            tamanioTotal = tamanio;
            Procesos = new List<Proceso>();
        }

        // Propiedad para acceder al tamaño total desde otras clases
        public int TamanioTotal
        {
            get { return tamanioTotal; }
        }

        public abstract bool AsignarMemoria(Proceso proceso);
        public abstract void LiberarMemoria(int idProceso);
    }
}
