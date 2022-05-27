using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace WebStore.Components;

//базовый класс ViewComponent дает доп инструменты, которые могут пригодиться
public class SectionsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
    //public async Task<IViewComponentResult> InvokeAsync() => View();
}
