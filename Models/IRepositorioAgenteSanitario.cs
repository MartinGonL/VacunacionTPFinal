using VacunacionTPFinal.Models;
using System.Collections.Generic;

namespace VacunacionEscolar.Repositorios
{
    public interface IRepositorioAgenteSanitario
    {
        int Alta(AgenteSanitario agente);
        int Baja(int id);
        int Modificacion(AgenteSanitario agente);
        AgenteSanitario ObtenerPorId(int id);
        IList<AgenteSanitario> ObtenerTodos();
        
        AgenteSanitario ObtenerPorUsuarioId(int idUsuario);
    }
}