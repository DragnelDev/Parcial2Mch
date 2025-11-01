using CadParcial2;
using ClnParcial2Mch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2Mch
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2MchEntities())
            {
                return context.Canal
                    .Where(x => x.estadoRegistro == 1)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}
