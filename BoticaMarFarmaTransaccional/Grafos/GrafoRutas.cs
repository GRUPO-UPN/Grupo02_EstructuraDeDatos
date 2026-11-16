using System;
using System.Collections.Generic;
using BoticaMarFarmaTransaccional.Colas;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional.Grafos
{
    // Grafo Ponderado No Dirigido implementado con Lista de Adyacencia.
    // Representa las sedes de Botica MarFarma (vértices) y las rutas entre ellas (aristas con km y minutos).
    public class GrafoRutas
    {
        private readonly List<VerticeGrafo> vertices;

        public GrafoRutas()
        {
            vertices = new List<VerticeGrafo>();
        }

        public List<VerticeGrafo> ObtenerVertices() => vertices;
        public int CantidadVertices => vertices.Count;

        // Cuenta las aristas únicas del grafo (en un grafo no dirigido cada ruta se guarda en ambos vértices).
        public int CantidadAristas
        {
            get
            {
                int suma = 0;
                foreach (var v in vertices)
                {
                    suma += v.Adyacentes.Count;
                }
                return suma / 2;
            }
        }

        // Busca la posición (índice) de un vértice por su código dentro de la lista.
        private int BuscarIndice(string codigo)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].Dato.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        // Agrega una nueva sede o punto de entrega si su código no existe todavía.
        public bool AgregarVertice(PuntoRed punto)
        {
            if (BuscarIndice(punto.Codigo) != -1)
            {
                return false;
            }

            vertices.Add(new VerticeGrafo(punto));
            return true;
        }

        // Crea o actualiza una ruta bidireccional entre dos sedes existentes.
        public bool AgregarArista(string codigoOrigen, string codigoDestino, double distanciaKm, int tiempoMin)
        {
            int idxOrigen = BuscarIndice(codigoOrigen);
            int idxDestino = BuscarIndice(codigoDestino);

            if (idxOrigen == -1 || idxDestino == -1 || idxOrigen == idxDestino || distanciaKm <= 0)
            {
                return false;
            }

            VerticeGrafo origen = vertices[idxOrigen];
            VerticeGrafo destino = vertices[idxDestino];

            GuardarAdyacencia(origen, destino, distanciaKm, tiempoMin);
            GuardarAdyacencia(destino, origen, distanciaKm, tiempoMin);
            return true;
        }

        private static void GuardarAdyacencia(VerticeGrafo desde, VerticeGrafo hacia, double km, int min)
        {
            foreach (var arista in desde.Adyacentes)
            {
                if (arista.Destino == hacia)
                {
                    arista.DistanciaKm = km;
                    arista.TiempoEstimadoMin = min;
                    return;
                }
            }

            desde.Adyacentes.Add(new AristaGrafo(hacia, km, min));
        }

        // Recorrido en Anchura (BFS): explora primero los vecinos más cercanos usando nuestra Cola FIFO.
        public List<VerticeGrafo> RecorrerBFS(string codigoInicio, List<string> trazas)
        {
            List<VerticeGrafo> resultado = new List<VerticeGrafo>();
            int idxInicio = BuscarIndice(codigoInicio);
            if (idxInicio == -1) return resultado;

            bool[] visitado = new bool[vertices.Count];
            Cola<int> cola = new Cola<int>();

            visitado[idxInicio] = true;
            cola.Enqueue(idxInicio);
            trazas.Add($"[BFS] Inicio desde [{vertices[idxInicio].Dato.Codigo}] {vertices[idxInicio].Dato.Nombre}");

            while (!cola.EstaVacia())
            {
                int actualIdx = cola.Dequeue();
                VerticeGrafo actual = vertices[actualIdx];
                resultado.Add(actual);
                trazas.Add($"  -> Visitado: [{actual.Dato.Codigo}] {actual.Dato.Nombre}");

                foreach (var arista in actual.Adyacentes)
                {
                    int vecinoIdx = vertices.IndexOf(arista.Destino);
                    if (!visitado[vecinoIdx])
                    {
                        visitado[vecinoIdx] = true;
                        cola.Enqueue(vecinoIdx);
                    }
                }
            }

            return resultado;
        }

        // Recorrido en Profundidad (DFS): avanza por cada rama hasta el fondo antes de retroceder.
        public List<VerticeGrafo> RecorrerDFS(string codigoInicio, List<string> trazas)
        {
            List<VerticeGrafo> resultado = new List<VerticeGrafo>();
            int idxInicio = BuscarIndice(codigoInicio);
            if (idxInicio == -1) return resultado;

            bool[] visitado = new bool[vertices.Count];
            trazas.Add($"[DFS] Inicio desde [{vertices[idxInicio].Dato.Codigo}] {vertices[idxInicio].Dato.Nombre}");
            DFSRecursivo(idxInicio, visitado, resultado, trazas);
            return resultado;
        }

        private void DFSRecursivo(int actualIdx, bool[] visitado, List<VerticeGrafo> resultado, List<string> trazas)
        {
            visitado[actualIdx] = true;
            VerticeGrafo actual = vertices[actualIdx];
            resultado.Add(actual);
            trazas.Add($"  -> Visitado: [{actual.Dato.Codigo}] {actual.Dato.Nombre}");

            foreach (var arista in actual.Adyacentes)
            {
                int vecinoIdx = vertices.IndexOf(arista.Destino);
                if (!visitado[vecinoIdx])
                {
                    DFSRecursivo(vecinoIdx, visitado, resultado, trazas);
                }
            }
        }

        // Algoritmo de Dijkstra: halla el camino con menor distancia total (km) entre origen y destino.
        public List<VerticeGrafo> CalcularRutaMasCorta(
            string codigoOrigen,
            string codigoDestino,
            out double distanciaTotal,
            out int tiempoTotal,
            List<string> trazas)
        {
            distanciaTotal = 0;
            tiempoTotal = 0;
            List<VerticeGrafo> camino = new List<VerticeGrafo>();

            int origen = BuscarIndice(codigoOrigen);
            int destino = BuscarIndice(codigoDestino);
            if (origen == -1 || destino == -1) return camino;

            int n = vertices.Count;
            double[] dist = new double[n];
            int[] tiempo = new int[n];
            int[] previo = new int[n];
            bool[] visitado = new bool[n];

            // Paso 1: inicializar distancias como infinito y predecesores en -1.
            for (int i = 0; i < n; i++)
            {
                dist[i] = double.MaxValue;
                tiempo[i] = int.MaxValue;
                previo[i] = -1;
            }

            dist[origen] = 0;
            tiempo[origen] = 0;
            trazas.Add($"[DIJKSTRA] Buscando ruta mínima de [{vertices[origen].Dato.Codigo}] hacia [{vertices[destino].Dato.Codigo}]");

            // Paso 2: en cada iteración elegir el vértice no visitado más cercano y relajar sus vecinos.
            for (int paso = 0; paso < n; paso++)
            {
                int u = -1;
                double menor = double.MaxValue;

                for (int i = 0; i < n; i++)
                {
                    if (!visitado[i] && dist[i] < menor)
                    {
                        menor = dist[i];
                        u = i;
                    }
                }

                if (u == -1) break;
                visitado[u] = true;
                trazas.Add($"  • Fijado [{vertices[u].Dato.Codigo}] con acumulado {dist[u]:F1} km");

                if (u == destino) break;

                foreach (var arista in vertices[u].Adyacentes)
                {
                    int v = vertices.IndexOf(arista.Destino);
                    if (visitado[v]) continue;

                    double nuevaDist = dist[u] + arista.DistanciaKm;
                    if (nuevaDist < dist[v])
                    {
                        dist[v] = nuevaDist;
                        tiempo[v] = tiempo[u] + arista.TiempoEstimadoMin;
                        previo[v] = u;
                        trazas.Add($"    ↳ Mejora hacia [{vertices[v].Dato.Codigo}]: {nuevaDist:F1} km (vía [{vertices[u].Dato.Codigo}])");
                    }
                }
            }

            if (dist[destino] == double.MaxValue)
            {
                trazas.Add("No existe ruta disponible entre los puntos seleccionados.");
                return camino;
            }

            // Paso 3: reconstruir el camino desde destino hasta origen usando el arreglo previo.
            int actual = destino;
            while (actual != -1)
            {
                camino.Insert(0, vertices[actual]);
                actual = previo[actual];
            }

            distanciaTotal = dist[destino];
            tiempoTotal = tiempo[destino];
            return camino;
        }

        // Comprueba si todos los puntos del grafo son alcanzables desde el primero.
        public bool EsGrafoConexo()
        {
            if (vertices.Count <= 1) return true;
            var visitados = RecorrerBFS(vertices[0].Dato.Codigo, new List<string>());
            return visitados.Count == vertices.Count;
        }

        // Devuelve la Lista de Adyacencia en texto para mostrar las conexiones en pantalla.
        public List<string> ObtenerListaAdyacencia()
        {
            List<string> lineas = new List<string>();
            foreach (var v in vertices)
            {
                if (v.Adyacentes.Count == 0)
                {
                    lineas.Add($"[{v.Dato.Codigo}] {v.Dato.Nombre} -> (Sin rutas)");
                }
                else
                {
                    List<string> destinos = new List<string>();
                    foreach (var a in v.Adyacentes)
                    {
                        destinos.Add($"{a.Destino.Dato.Codigo} ({a.DistanciaKm:F1} km)");
                    }
                    lineas.Add($"[{v.Dato.Codigo}] {v.Dato.Nombre} -> {string.Join(" | ", destinos)}");
                }
            }
            return lineas;
        }

        // Algoritmo de Prim: construye el Árbol de Expansión Mínima (MST) que conecta toda la red con el menor kilometraje total.
        public double CalcularArbolExpansionMinimaPrim(List<string> trazas)
        {
            int n = vertices.Count;
            if (n == 0) return 0;

            bool[] enArbol = new bool[n];
            double[] costoMin = new double[n];
            int[] padre = new int[n];

            for (int i = 0; i < n; i++)
            {
                costoMin[i] = double.MaxValue;
                padre[i] = -1;
            }

            costoMin[0] = 0;
            double totalKm = 0;
            trazas.Add($"[PRIM] Construyendo Árbol de Expansión Mínima desde [{vertices[0].Dato.Codigo}]");

            for (int paso = 0; paso < n; paso++)
            {
                int u = -1;
                double menor = double.MaxValue;
                for (int i = 0; i < n; i++)
                {
                    if (!enArbol[i] && costoMin[i] < menor)
                    {
                        menor = costoMin[i];
                        u = i;
                    }
                }

                if (u == -1) break;
                enArbol[u] = true;

                if (padre[u] != -1)
                {
                    totalKm += costoMin[u];
                    trazas.Add($"  + Enlace óptimo: [{vertices[padre[u]].Dato.Codigo}] <-> [{vertices[u].Dato.Codigo}] ({costoMin[u]:F1} km)");
                }

                foreach (var arista in vertices[u].Adyacentes)
                {
                    int v = vertices.IndexOf(arista.Destino);
                    if (!enArbol[v] && arista.DistanciaKm < costoMin[v])
                    {
                        costoMin[v] = arista.DistanciaKm;
                        padre[v] = u;
                    }
                }
            }

            trazas.Add($"Total mínimo para conectar la red completa (Prim): {totalKm:F1} km");
            return totalKm;
        }

        // Genera la Matriz de Adyacencia ponderada (filas = origen, columnas = destino) para inspección tabular.
        public List<string> ObtenerMatrizAdyacenciaTexto()
        {
            List<string> filas = new List<string>();
            int n = vertices.Count;
            if (n == 0) return filas;

            string cabecera = "       ";
            for (int j = 0; j < n; j++)
            {
                cabecera += $" {vertices[j].Dato.Codigo,6}";
            }
            filas.Add(cabecera);
            filas.Add(new string('-', cabecera.Length));

            for (int i = 0; i < n; i++)
            {
                string linea = $"{vertices[i].Dato.Codigo,-6}|";
                for (int j = 0; j < n; j++)
                {
                    double peso = 0;
                    foreach (var arista in vertices[i].Adyacentes)
                    {
                        if (arista.Destino == vertices[j])
                        {
                            peso = arista.DistanciaKm;
                            break;
                        }
                    }
                    linea += peso > 0 ? $" {peso,6:F1}" : "      -";
                }
                filas.Add(linea);
            }

            return filas;
        }
    }
}
