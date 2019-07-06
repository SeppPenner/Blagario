using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blagario.elements
{
    public class World : AgarElement
    {
        public List<AgarElement> Elements;
        public const long MaxMass = 60 * 1000;
        public const long MaxViruses = 100;        
        public const long MaxPellets = 1000;        

        public override string CssStyle( Eyeglass c ) =>$@"
            top: {c.YGame2Physics(0)}px ;
            left: {c.XGame2Physics(0)}px;
            width: {(X * c.Cell.Zoom).ToString()}px ;
            height: {(Y * c.Cell.Zoom).ToString()}px ; 
            "; 

        public World(Universe universe)
        {
            this.X = 1000;
            this.Y = 1000;
            Elements = new List<AgarElement>();
            Universe = universe;
        }

        public long TotalMass => Elements.Sum( x=> x.Mass );
        public IEnumerable<Virus> Viruses => Elements.Where(x => x.ElementType == ElementType.Virus ).Select(x=>x as Virus);
        public IEnumerable<Pellet> Pellets => Elements.Where(x => x.ElementType == ElementType.Pellet ).Select(x=>x as Pellet);

        public override async Task Tic()
        {
            List<AgarElement> currentElements;
            lock(this.Elements) currentElements = this.Elements.ToList();
            CheckIfWoldNedsMoreViruses(currentElements);            
            CheckIfWoldNedsMorePellets(currentElements);            

            lock(this.Elements) currentElements = this.Elements.ToList();
            foreach (var e in currentElements) await e.Tic();

            await base.Tic();
        }

        private void CheckIfWoldNedsMorePellets(List<AgarElement> currentElements)
        {
            var nPellets = currentElements.Where(x=>x.ElementType == ElementType.Pellet).Count();
            var mass = currentElements.Sum(e=>e.Mass);
            lock(this.Elements)
            while( nPellets < MaxPellets && mass < MaxMass )
            {
                var e = Pellet.CreatePellet(this.Universe);        
                mass += e.Mass;        
                nPellets++;
            }            
        }

        private void CheckIfWoldNedsMoreViruses(List<AgarElement> currentElements)
        {
            var nViruses = currentElements.Where(x=>x.ElementType == ElementType.Pellet).Count();
            var mass = currentElements.Sum(e=>e.Mass);
            lock(this.Elements)
            while( nViruses < MaxViruses && mass < MaxMass )
            {
                var e = Virus.CreateVirus(this.Universe);
                mass += e.Mass;        
                nViruses++;                
            }
        }
    }
}
