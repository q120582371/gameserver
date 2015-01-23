using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public interface IObjectId
    {
        long Id { get; set; }
    }

    public class GameEntity : IObjectId
    {
        public long Id { get; set; }
    }
}
