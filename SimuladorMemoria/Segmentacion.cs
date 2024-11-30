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
            segmentos = new List<(int idProceso, int inicio, int tamanio)>();
        }

        public override bool AsignarMemoria(Proceso proceso)
        {
            var espaciosLibres = ObtenerEspaciosLibres();
            var espacioAdecuado = espaciosLibres.FirstOrDefault(e => e.tamanio >= proceso.Tamanio);

            if (espacioAdecuado == default)
            {
                Console.WriteLine($"Error: No hay suficiente espacio para asignar el proceso {proceso.Id}.");
                return false;
            }

            // Asignar el segmento
            segmentos.Add((proceso.Id, espacioAdecuado.inicio, proceso.Tamanio));
            Procesos.Add(proceso);

            // Actualizar la lista de segmentos libres
            var nuevoEspacioInicio = espacioAdecuado.inicio + proceso.Tamanio;
            var nuevoEspacioTamanio = espacioAdecuado.tamanio - proceso.Tamanio;

            segmentosLibres.RemoveAll(s => s.inicio == espacioAdecuado.inicio); 
            if (nuevoEspacioTamanio > 0)
            {
                segmentosLibres.Add(new SegmentoLibre(nuevoEspacioInicio, nuevoEspacioTamanio));
            }

            CompactarMemoria();
            return true;
        }

        public List<(int inicio, int tamanio)> ObtenerEspaciosLibres()
        {
            var espaciosLibres = new List<(int inicio, int tamanio)>();
            int inicioLibre = 0;

            var segmentosOrdenados = segmentos.OrderBy(s => s.inicio).ToList();

            // Identificar los espacios libres entre segmentos
            foreach (var segmento in segmentosOrdenados)
            {
                if (inicioLibre < segmento.inicio)
                {
                    // Añadir espacio libre antes del segmento actual
                    espaciosLibres.Add((inicioLibre, segmento.inicio - inicioLibre));
                }
                // Actualizar el inicio libre al final del segmento actual
                inicioLibre = segmento.inicio + segmento.tamanio;
            }

            // Espacio libre al final de la memoria
            if (inicioLibre < TamanioTotal)
            {
                espaciosLibres.Add((inicioLibre, TamanioTotal - inicioLibre));
            }

            return espaciosLibres;
        }

        public override void LiberarMemoria(int idProceso)
        {
            // Buscar y eliminar segmentos del proceso
            var segmentosLiberados = segmentos.Where(s => s.idProceso == idProceso).ToList();
            foreach (var segmento in segmentosLiberados)
            {
                segmentos.Remove(segmento);

                // Agregar el espacio que se libro como un nuevo segmento libre
                segmentosLibres.Add(new SegmentoLibre(segmento.inicio, segmento.tamanio));
            }

            // Fusionar segmentos libres contiguos
            CompactarMemoria();

            // Eliminar el proceso de la lista
            Procesos.RemoveAll(p => p.Id == idProceso);
            Console.WriteLine($"Memoria del proceso {idProceso} liberada exitosamente.");
        }

        public void MostrarEstadoMemoria()
        {
            Console.WriteLine("========== Estado de la Memoria ==========");
            Console.WriteLine($"Tamaño Total: {TamanioTotal}");

            if (segmentos.Count == 0)
            {
                Console.WriteLine("No hay segmentos asignados.");
            }
            else
            {
                Console.WriteLine("Segmentos ocupados:");
                foreach (var segmento in segmentos.OrderBy(s => s.inicio))
                {
                    Console.WriteLine($"- Proceso {segmento.idProceso}: Inicio {segmento.inicio}, Tamaño {segmento.tamanio}");
                }
            }

            var espaciosLibres = ObtenerEspaciosLibres();
            if (espaciosLibres.Count > 0)
            {
                Console.WriteLine("\nFragmentación externa (segmentos libres):");
                foreach (var espacio in espaciosLibres)
                {
                    Console.WriteLine($"- Desde posición {espacio.inicio} hasta {espacio.inicio + espacio.tamanio}: {espacio.tamanio}");
                }
            }
            else
            {
                Console.WriteLine("\nNo hay fragmentación externa.");
            }

            Console.Write("\nMemoria Física:\n[");
            int posicionActual = 0;
            foreach (var segmento in segmentos.OrderBy(s => s.inicio))
            {
                if (posicionActual < segmento.inicio)
                {
                    Console.Write(new string('░', segmento.inicio - posicionActual));
                }
                Console.Write(new string('█', segmento.tamanio));
                posicionActual = segmento.inicio + segmento.tamanio;
            }
            if (posicionActual < TamanioTotal)
            {
                Console.Write(new string('░', TamanioTotal - posicionActual));
            }
            Console.WriteLine("]");
        }

        private List<SegmentoLibre> segmentosLibres = new List<SegmentoLibre>();

        public void CompactarMemoria()
        {
            // Ordenar los segmentos libres por posición inicial
            segmentosLibres = segmentosLibres.OrderBy(s => s.inicio).ToList();

            for (int i = 0; i < segmentosLibres.Count - 1; i++)
            {
                var actual = segmentosLibres[i];
                var siguiente = segmentosLibres[i + 1];

                // Si el final de un segmento coincide con el inicio del siguiente, se fusionan
                if (actual.inicio + actual.tamanio == siguiente.inicio)
                {
                    actual.tamanio += siguiente.tamanio;
                    segmentosLibres[i] = actual;
                    segmentosLibres.RemoveAt(i + 1);
                    i--;
                }
            }
        }

        public List<(int idProceso, int inicio, int tamanio)> ObtenerSegmentos()
        {
            return segmentos;
        }

    }
}
