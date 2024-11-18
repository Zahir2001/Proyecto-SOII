using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class Segmentacion: Memoria
    {
        private List<(int idProceso, int inicio, int tamanio)> segmentos;

        public Segmentacion(int tamanioTotal) : base(tamanioTotal)
        {
            segmentos = new List<(int, int, int)>();
        }

        public override bool AsignarMemoria(Proceso proceso)
        {
            int espacioLibre = 0;
            int inicioSegmento = -1;

            foreach (var segmento in segmentos.OrderBy(s => s.inicio))
            {
                if (segmento.inicio - espacioLibre >= proceso.Tamanio)
                {
                    inicioSegmento = espacioLibre;
                    break;
                }
                espacioLibre = segmento.inicio + segmento.tamanio;
            }

            if (inicioSegmento == -1 && TamanioTotal - espacioLibre >= proceso.Tamanio)
            {
                inicioSegmento = espacioLibre;
            }

            if (inicioSegmento == -1) return false;

            segmentos.Add((proceso.Id, inicioSegmento, proceso.Tamanio));
            Procesos.Add(proceso);
            return true;
        }

        public override void LiberarMemoria(int idProceso)
        {
            segmentos.RemoveAll(s => s.idProceso == idProceso);
            Procesos.RemoveAll(p => p.Id == idProceso);
        }

        public List<(int idProceso, int inicio, int tamanio)> ObtenerSegmentosPorProceso()
        {
            return segmentos;
        }
    }
}
