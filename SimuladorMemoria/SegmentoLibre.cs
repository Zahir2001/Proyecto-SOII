using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class SegmentoLibre
    {
        public int inicio { get; set; }
        public int tamanio { get; set; }

        public SegmentoLibre(int inicio, int tamanio)
        {
            this.inicio = inicio;
            this.tamanio = tamanio;
        }
    }
}
