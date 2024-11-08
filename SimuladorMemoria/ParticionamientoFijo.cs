using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class ParticionamientoFijo : Memoria
    {
        private int TamanioParticion;
        private bool[] Particiones;

        public ParticionamientoFijo(int tamanio, int tamanioParticion) : base(tamanio)
        {
            TamanioParticion = tamanioParticion;
            Particiones = new bool[tamanio / tamanioParticion];
        }

        public override bool AsignarMemoria(Proceso proceso)
        {
            for (int i = 0; i < Particiones.Length; i++)
            {
                if (!Particiones[i] && proceso.Tamanio <= TamanioParticion)
                {
                    Particiones[i] = true;
                    Procesos.Add(proceso);
                    return true;
                }
            }
            return false;
        }

        public override void LiberarMemoria(int idProceso)
        {
            var proceso = Procesos.Find(p => p.Id == idProceso);
            if (proceso != null)
            {
                int index = Procesos.IndexOf(proceso) / TamanioParticion;
                Particiones[index] = false;
                Procesos.Remove(proceso);
            }
        }
    }
}
