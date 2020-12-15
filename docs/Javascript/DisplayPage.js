var LoadPage = function()
{
    const urlParams = new URLSearchParams(window.location.search);
    const PagePath = urlParams.get('PagePath');
    var MenuId = "Contenu";
    var MenuDiv = document.getElementById(MenuId);
    var TextToInsert = `<iframe frameborder = "0" width="100%" height="100%" src="Mods/${PagePath}"></iframe>`;
    MenuDiv.insertAdjacentHTML('beforeend', TextToInsert);
}