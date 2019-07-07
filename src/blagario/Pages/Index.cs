using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blagario.elements;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blagario
{

    public abstract class BaseIndex : ComponentBase, IDisposable
    {
        [Inject] protected Universe Universe {get; set; }        
        [Inject] protected Cell MyCell {get; set; }
        [Inject] protected Eyeglass Eyeglass {get; set; }
        [Inject] protected IJSRuntime JsRuntime  {get; set; }

        private bool Rendered = false;
        public void Dispose()
        {
        }

        protected List<AgarElement> VisibleElements = new List<AgarElement>();

        protected bool IsDirty = false;

        protected override void OnInit()
        {      
            Universe.World.OnTicReached += (a,b)=> Invoke( () =>{
                IsDirty = true;
            });   

            Invoke(
                async () =>
                {
                    while (true)
                    {
                        await Task.Delay(20);
                        if (! IsDirty) continue;
                        VisibleElements = Universe.World.Elements.Where( e=> Eyeglass.OnArea(e) ).ToList();
                        StateHasChanged();                    
                    }
                });   

        }
        
        protected void TrackMouse(UIMouseEventArgs e)
        {
            var bx = Eyeglass.XPysics2Game(e.ClientX);
            var by = Eyeglass.YPysics2Game(e.ClientY);
            MyCell.PointTo( bx, by);
        }

        protected async override Task OnAfterRenderAsync()
        {
            if (!Rendered) 
            {
                Rendered = true;
                await OnAfterFirstRenderAsync();
            }            
        }
        protected async Task OnAfterFirstRenderAsync()
        {
            await Eyeglass.CheckVisibleArea(JsRuntime);
        }
    }
}