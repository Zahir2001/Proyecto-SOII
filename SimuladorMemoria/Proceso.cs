using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMemoria
{
    public class Proceso
    {
        public int Id { get; }
        public int Tamanio { get; }

        public Proceso(int id, int tamanio)
        {
            Id = id;
            Tamanio = tamanio;
        }
    }
}
