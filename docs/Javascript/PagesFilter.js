var ChoosePagesFilter = function ()
{
    if (OrderBy.selectedIndex >= 0)
	{
		window.location = location.protocol + '//' + location.host + location.pathname +
		"?OrderBy=" + OrderBy.value + "&ModsFilter=" + ModsFilter.value + "&PagesFilter=" + PagesFilter.value ;
	}
}