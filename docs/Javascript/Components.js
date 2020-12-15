var components = [
    {
        Name: "Battle",
        Components: ["Battle Effects"],
        GUIs: []
    },
    {
        Name: "Conquest",
        Components: ["Conquest AI", "Conquest Map"],
        GUIs: ["Conquest Unit"],
    },
    {
        Name: "Core",
        Components: ["Animation Screen", "Base Editor", "Character", "Core AI", "Core Scripts", "Cutscene", "Formula Parser", "Magic", "Unit"],
        GUIs: ["AI", "Animation background 2D", "Animation background 3D", "Animation", "Attack", "Bitmap animation", "Character Skill", "Character", "Character spirits", "Cutscene",
                "Image viewer", "Music player", "Roster editor", "System list", "Tileset", "Unit ability", "Unit consumable part", "Unit standard part", "Visual novel"],
    },
    {
        Name: "Deathmatch",
        Components: ["Deathmatch AI", "Deathmatch Skill Requirements", "Formula Parser"],
        GUIs: ["Combining unit", "Deathmatch map", "Hub unit", "Multi-form unit", "Normal unit", "Transforming unit"]
    },
    {
        Name: "Triple Thunder",
        Components: ["Triple Thunder AI", "Triple Thunder Map"],
        GUIs: []
    },
    {
        Name: "World",
        Components: ["World Map"],
        GUIs: []
    }
  ];
  
  var FillMenu = function()
  {
    const urlParams = new URLSearchParams(window.location.search);
    const OrderBy = urlParams.get('OrderBy');
    const ModsFilter = urlParams.get('ModsFilter');
    const PagesFilter = urlParams.get('PagesFilter');
    var MenuId = "Components";
    var MenuDiv = document.getElementById(MenuId);
    var MenuTemplate = `<td width='25%'>
    <div>{Menu Name}</div>
    <a href='DisplayPage.html?PagePath={Mod Name}/{Category Name}/{Menu Name}.html'>
        <img src='Images/Default.png' alt='Photo profile' align='middle'>
    </a>
    </td>`;
    
    var ElementCounter = -1;
    var TextToInsert = "<tr>";

    components.forEach(function(Mod) {

        if (OrderBy === "Mods")
        {
            TextToInsert += "<table class='dimensionTable' id='Pages Table'>";
			TextToInsert += "<caption>" + Mod.Name + "</caption>";
            TextToInsert += "<tr>";
        }
        if (ModsFilter !== "*" && Mod.Name !== ModsFilter)
        {
            return;
        }

        if (PagesFilter === "*" || PagesFilter === "Components")
        {
            Mod.Components.forEach(function(ItemName) {

                ElementCounter += 1;

                if (ElementCounter === 4)
                {
                    TextToInsert += "</tr><tr>";
                    ElementCounter = 0;
                }
                
                TextToInsert += MenuTemplate.replaceAll("{Menu Name}", ItemName).replace("{Mod Name}", Mod.Name).replace("{Category Name}", "Components");
            });
        }
        if (PagesFilter === "*" || PagesFilter === "GUIs")
        {
            Mod.GUIs.forEach(function(ItemName) {

                ElementCounter += 1;

                if (ElementCounter === 4)
                {
                    TextToInsert += "</tr><tr>";
                    ElementCounter = 0;
                }

                TextToInsert += MenuTemplate.replaceAll("{Menu Name}", ItemName).replace("{Mod Name}", Mod.Name).replace("{Category Name}", "GUIs");
            });
        }
        if (OrderBy === "Mods")
        {
            TextToInsert +=
            `	</tr>
            </table>`;
        }
    });

    TextToInsert += "</tr>";
    MenuDiv.insertAdjacentHTML('beforeend', TextToInsert);
  }