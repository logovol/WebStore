using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace WebStore.Components;

public class BrandsViewComponent : ViewComponent
{
    // можно выбирать представление, если их несколько
    // public IViewComponentResult Invoke() => View("Name");
    public IViewComponentResult Invoke() => View();
}
