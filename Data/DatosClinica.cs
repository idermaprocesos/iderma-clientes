using IdermaClinica.Models;

namespace IdermaClinica.Data;

public sealed partial class ClinicaStore
{
    private void Sembrar()
    {
        var hoy = DateTime.Today;
        var clientes = new List<Cliente>();
        var operaciones = new List<Operacion>();
        var cuidados = new List<Cuidado>();
        var ventas = new List<Venta>();

        Cliente Ficha(int id, string nombre, int edad, string telefono, string correo, DateTime alta, string motivo, string zona, string notas)
        {
            var cliente = new Cliente
            {
                Id = id,
                Nombre = nombre,
                Edad = edad,
                Telefono = telefono,
                Correo = correo,
                FechaAlta = alta,
                Motivo = motivo,
                Zona = zona,
                Notas = notas
            };
            clientes.Add(cliente);
            return cliente;
        }

        Ficha(1, "Mariana López Herrera", 34, "987 112 334", "mariana.lopez@correo.com", hoy.AddMonths(-8),
            "Alopecia androgenética femenina", "Difusa",
            "Prefiere las visitas por la mañana. Le cuesta acordarse de la crema de noche.");
        Ficha(2, "Carlos Herrera Vidal", 41, "945 445 901", "carlos.herrera@correo.com", hoy.AddMonths(-10),
            "Norwood III, entradas", "Entradas y frontal",
            "Trasplante reciente. Constante con la medicación oral; se salta algún día el minoxidil.");
        Ficha(3, "Lucía Fernández Soto", 29, "912 908 221", "lucia.fernandez@correo.com", hoy.AddMonths(-3),
            "Efluvio postparto", "Difusa",
            "En revisión analítica por hierro bajo. La crema calmante ya la terminó.");
        Ficha(4, "Andrés Molina Ruiz", 52, "998 330 118", "andres.molina@correo.com", hoy.AddMonths(-13),
            "Norwood V, coronilla", "Coronilla",
            "La coronilla responde, pero se retrasa al renovar la finasterida.");
        Ficha(5, "Sofía Ramírez Peña", 37, "956 221 670", "sofia.ramirez@correo.com", hoy.AddMonths(-6),
            "Pérdida de densidad frontal", "Frontal",
            "Quiere densidad sin cirugía por ahora. Retiró la crema y casi no la usa.");
        Ficha(6, "Diego Vargas León", 29, "993 774 015", "diego.vargas@correo.com", hoy.AddMonths(-2),
            "Barba con calvas", "Barba",
            "Trabaja de cara al público y pregunta mucho por los plazos de la barba.");
        Ficha(7, "Elena Castillo Mora", 46, "964 019 442", "elena.castillo@correo.com", hoy.AddMonths(-8),
            "Cicatriz de trasplante previo", "Línea frontal",
            "Micropigmentación para camuflar una cicatriz antigua. La pauta ya está cerrada.");
        Ficha(8, "Pablo Núñez Gil", 33, "921 552 903", "pablo.nunez@correo.com", hoy.AddDays(-20),
            "Norwood II, línea frontal", "Frontal",
            "Primera visita. Sale con finasterida y minoxidil. Aún no retira el frasco de minoxidil.");

        Operacion NuevaOp(int clienteId, string tipo, string categoria, DateTime fecha, string medico, string zona, string detalle, decimal importe, bool seguimiento)
        {
            var cliente = clientes.First(c => c.Id == clienteId);
            var operacion = new Operacion
            {
                Id = operaciones.Count + 1,
                ClienteId = clienteId,
                ClienteNombre = cliente.Nombre,
                Tipo = tipo,
                Categoria = categoria,
                Fecha = fecha,
                Medico = medico,
                Zona = zona,
                Detalle = detalle,
                Importe = importe,
                EnSeguimiento = seguimiento
            };
            operaciones.Add(operacion);
            return operacion;
        }

        void Pautar(Operacion op, TipoCuidado tipo, string nombre, string indicacion, int dias, DateTime inicio, int cumplidos, DateTime? ultimo, decimal precio, string estadoVenta, int veces, string periodo)
        {
            var cuidado = new Cuidado
            {
                Id = cuidados.Count + 1,
                OperacionId = op.Id,
                ClienteId = op.ClienteId,
                ClienteNombre = op.ClienteNombre,
                OperacionCorta = op.Tipo,
                OperacionResumen = $"{op.Tipo} · {op.FechaTexto}",
                Tipo = tipo,
                Nombre = nombre,
                Indicacion = indicacion,
                Veces = veces,
                Periodo = periodo,
                DiasTratamiento = dias,
                FechaInicio = inicio,
                DiasCumplidos = cumplidos,
                UltimoRegistro = ultimo
            };
            cuidados.Add(cuidado);
            op.Cuidados.Add(cuidado);

            if (estadoVenta == "Pendiente")
                cuidado.AvisoVenta = "Hay una venta pendiente de este producto. El paciente todavía no lo ha retirado.";

            ventas.Add(new Venta
            {
                Id = ventas.Count + 1,
                ClienteId = op.ClienteId,
                ClienteNombre = op.ClienteNombre,
                Cuidado = cuidado,
                CuidadoId = cuidado.Id,
                Producto = nombre,
                TipoProducto = cuidado.TipoTexto,
                OperacionCorta = op.Tipo,
                Cantidad = 1,
                PrecioUnitario = precio,
                Fecha = estadoVenta == "Pendiente" ? hoy.AddDays(-1) : inicio,
                Estado = estadoVenta
            });
        }

        var carlos = NuevaOp(2, "Trasplante capilar FUE", "Trasplante", hoy.AddDays(-40),
            "Dra. Inés Valverde", "Frontal y entradas", "2.450 unidades foliculares", 4800, true);
        Pautar(carlos, TipoCuidado.Medicacion, "Cefalexina 500 mg", "1 comprimido cada 8 horas, con comida.", 7, carlos.Fecha, 7, carlos.Fecha.AddDays(6), 8, "Cobrada", 3, "8 horas");
        Pautar(carlos, TipoCuidado.Medicacion, "Paracetamol 1 g", "1 comprimido si hay molestia, máximo 3 al día.", 5, carlos.Fecha, 5, carlos.Fecha.AddDays(4), 4, "Cobrada", 3, "día");
        Pautar(carlos, TipoCuidado.Crema, "Mupirocina zona donante", "Capa fina en la zona donante, mañana y noche.", 10, carlos.Fecha, 10, carlos.Fecha.AddDays(9), 9, "Cobrada", 2, "día");
        Pautar(carlos, TipoCuidado.Crema, "Espuma cicatrizante post-injerto", "Sobre los injertos, sin frotar, una vez al día.", 14, carlos.Fecha.AddDays(1), 14, carlos.Fecha.AddDays(14), 18, "Cobrada", 1, "día");
        Pautar(carlos, TipoCuidado.Medicacion, "Minoxidil 5% solución", "1 ml en cuero cabelludo seco, mañana y noche. Desde el día 15.", 90, carlos.Fecha.AddDays(15), 22, hoy.AddDays(-1), 32, "Cobrada", 2, "día");
        Pautar(carlos, TipoCuidado.Medicacion, "Finasterida 1 mg", "1 comprimido por la noche.", 180, carlos.Fecha, 38, hoy.AddDays(-1), 28, "Cobrada", 1, "noche");

        var andres = NuevaOp(4, "Trasplante capilar FUE", "Trasplante", hoy.AddDays(-78),
            "Dr. Hugo Salinas", "Coronilla y vértex", "3.100 unidades foliculares", 5600, true);
        Pautar(andres, TipoCuidado.Medicacion, "Cefalexina 500 mg", "1 comprimido cada 8 horas, con comida.", 7, andres.Fecha, 7, andres.Fecha.AddDays(6), 8, "Cobrada", 3, "8 horas");
        Pautar(andres, TipoCuidado.Crema, "Mupirocina zona donante", "Capa fina en la zona donante, mañana y noche.", 10, andres.Fecha, 10, andres.Fecha.AddDays(9), 9, "Cobrada", 2, "día");
        Pautar(andres, TipoCuidado.Medicacion, "Minoxidil 5% solución", "1 ml mañana y noche desde el día 15.", 90, andres.Fecha.AddDays(15), 61, hoy.AddDays(-1), 32, "Cobrada", 2, "día");
        Pautar(andres, TipoCuidado.Medicacion, "Finasterida 1 mg", "1 comprimido por la noche. Renovar la caja cada 30 días.", 180, andres.Fecha, 70, hoy.AddDays(-4), 28, "Pendiente", 1, "noche");
        Pautar(andres, TipoCuidado.Crema, "Crema regeneradora nocturna", "Capa fina por la noche en coronilla y vértex.", 60, andres.Fecha.AddDays(14), 52, hoy.AddDays(-6), 36, "Cobrada", 1, "noche");

        var elena = NuevaOp(7, "Micropigmentación capilar", "Micropigmentación", hoy.AddDays(-93),
            "Raúl Ibáñez", "Línea frontal y cicatriz", "Camuflaje de cicatriz y línea frontal", 890, false);
        Pautar(elena, TipoCuidado.Crema, "Crema cicatrizante", "Capa fina sobre la zona pigmentada, dos veces al día.", 14, elena.Fecha, 14, elena.Fecha.AddDays(13), 14, "Cobrada", 2, "día");
        Pautar(elena, TipoCuidado.Crema, "Crema hidratante con SPF", "Por la mañana, durante 30 días. Evitar sauna la primera semana.", 30, elena.Fecha.AddDays(1), 30, elena.Fecha.AddDays(30), 24, "Cobrada", 1, "día");

        var marianaPrevia = NuevaOp(1, "Mesoterapia capilar", "Mesoterapia", hoy.AddDays(-47),
            "Dra. Inés Valverde", "Cuero cabelludo", "Sesión 3 de 6", 280, false);
        Pautar(marianaPrevia, TipoCuidado.Medicacion, "Minoxidil 2% solución", "1 ml por la noche. Pauta de esa sesión, ya relevada por la siguiente.", 30, marianaPrevia.Fecha, 30, marianaPrevia.Fecha.AddDays(29), 29, "Cobrada", 1, "noche");

        var diego = NuevaOp(6, "Trasplante de barba FUE", "Trasplante", hoy.AddDays(-24),
            "Dra. Inés Valverde", "Mentón y patillas", "900 unidades foliculares", 2200, true);
        Pautar(diego, TipoCuidado.Medicacion, "Cefalexina 500 mg", "1 comprimido cada 8 horas, con comida.", 7, diego.Fecha, 7, diego.Fecha.AddDays(6), 8, "Cobrada", 3, "8 horas");
        Pautar(diego, TipoCuidado.Crema, "Mupirocina en la barba", "Capa fina en la zona tratada, mañana y noche.", 10, diego.Fecha, 9, hoy.AddDays(-1), 11, "Cobrada", 2, "día");
        Pautar(diego, TipoCuidado.Crema, "Loción hidratante de barba", "Sobre la barba limpia, mañana y noche, sin frotar los injertos.", 21, diego.Fecha.AddDays(1), 14, hoy.AddDays(-3), 26, "Pendiente", 2, "día");
        Pautar(diego, TipoCuidado.Medicacion, "Minoxidil 5% barba", "1 ml desde el día 15, solo por la noche.", 90, diego.Fecha.AddDays(15), 7, hoy.AddDays(-1), 32, "Cobrada", 1, "noche");

        var mariana = NuevaOp(1, "Mesoterapia capilar", "Mesoterapia", hoy.AddDays(-19),
            "Dra. Inés Valverde", "Cuero cabelludo", "Sesión 4 de 6", 280, true);
        Pautar(mariana, TipoCuidado.Medicacion, "Minoxidil 2% solución", "1 ml por la noche, sobre cuero cabelludo seco.", 90, mariana.Fecha, 18, hoy.AddDays(-1), 29, "Cobrada", 1, "noche");
        Pautar(mariana, TipoCuidado.Crema, "Crema de péptidos", "Capa fina por la noche. Es el cuidado que más se le olvida.", 60, mariana.Fecha, 11, hoy.AddDays(-5), 42, "Pendiente", 1, "noche");
        Pautar(mariana, TipoCuidado.Medicacion, "Biotina y zinc", "1 cápsula con el desayuno.", 90, mariana.Fecha, 20, hoy, 22, "Cobrada", 1, "día");

        var pablo = NuevaOp(8, "Inicio de pauta médica", "Consulta", hoy.AddDays(-13),
            "Dr. Hugo Salinas", "Línea frontal", "Valoración y pauta domiciliaria", 120, true);
        Pautar(pablo, TipoCuidado.Medicacion, "Finasterida 1 mg", "1 comprimido por la noche.", 90, pablo.Fecha, 13, hoy.AddDays(-1), 28, "Cobrada", 1, "noche");
        Pautar(pablo, TipoCuidado.Medicacion, "Minoxidil 5% solución", "1 ml mañana y noche. No ha retirado el frasco.", 90, pablo.Fecha, 9, hoy.AddDays(-4), 32, "Pendiente", 2, "día");
        Pautar(pablo, TipoCuidado.Crema, "Crema de ketoconazol 2%", "Capa fina, 3 noches por semana.", 30, pablo.Fecha, 12, hoy.AddDays(-1), 16, "Cobrada", 3, "semana");

        var lucia = NuevaOp(3, "PRP capilar", "PRP", hoy.AddDays(-11),
            "Dr. Hugo Salinas", "Difuso", "Sesión 2 de 3", 320, true);
        Pautar(lucia, TipoCuidado.Medicacion, "Minoxidil 2% solución", "1 ml por la noche.", 90, lucia.Fecha, 11, hoy.AddDays(-1), 29, "Cobrada", 1, "noche");
        Pautar(lucia, TipoCuidado.Crema, "Crema calmante post-PRP", "Capa fina si hay enrojecimiento, durante 7 días.", 7, lucia.Fecha, 7, lucia.Fecha.AddDays(6), 16, "Cobrada", 2, "día");
        Pautar(lucia, TipoCuidado.Medicacion, "Hierro y vitamina D", "1 sobre al día, con la comida. Pendiente de la analítica.", 60, lucia.Fecha, 4, hoy.AddDays(-6), 19, "Pendiente", 1, "día");

        var sofia = NuevaOp(5, "Mesoterapia y PRP", "PRP", hoy.AddDays(-6),
            "Dra. Inés Valverde", "Frontal", "Sesión 1 de 4", 450, true);
        Pautar(sofia, TipoCuidado.Medicacion, "Dutasterida 0,5 mg", "1 cápsula al día, siempre a la misma hora.", 90, sofia.Fecha, 5, hoy.AddDays(-1), 34, "Cobrada", 1, "día");
        Pautar(sofia, TipoCuidado.Medicacion, "Minoxidil 5% espuma", "Medio dosificador por la noche. Hoy ya está aplicado.", 90, sofia.Fecha, 7, hoy, 33, "Cobrada", 1, "noche");
        Pautar(sofia, TipoCuidado.Crema, "Crema densificadora nocturna", "Capa fina en la línea frontal, por la noche.", 30, sofia.Fecha, 2, hoy.AddDays(-3), 48, "Cobrada", 1, "noche");

        foreach (var cliente in clientes)
            Clientes.Add(cliente);
        foreach (var operacion in operaciones.OrderByDescending(o => o.Fecha))
            Operaciones.Add(operacion);
        foreach (var cuidado in cuidados)
            Cuidados.Add(cuidado);
        foreach (var venta in ventas.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id))
            Ventas.Add(venta);

        ActualizarFichas();
        foreach (var operacion in Operaciones.Take(5))
            OperacionesRecientes.Add(operacion);
    }
}
