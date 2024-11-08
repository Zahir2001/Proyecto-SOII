using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class ParticionamientoDinamico : Memoria
    {
        public ParticionamientoDinamico(int tamanio) : base(tamanio) { }

        public override bool AsignarMemoria(Proceso proceso)
        {
            int espacioDisponible = TamanioTotal - Procesos.Sum(p => p.Tamanio);
            if (proceso.Tamanio <= espacioDisponible)
            {
                Procesos.Add(proceso);
                return true;
            }
            return false;
        }

        public override void LiberarMemoria(int idProceso)
        {
            var proceso = Procesos.Find(p => p.Id == idProceso);
            if (proceso != null)
            {
                Procesos.Remove(proceso);
            }
        }
    }
}
