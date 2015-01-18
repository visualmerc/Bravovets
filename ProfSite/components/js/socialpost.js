// Placeholder for social post js
console.log("here now");
$('button.add-to-calendar').click(function() {
	var toggleVal = $(this).attr("data-toggle");

	if (toggleVal === "true") {
		$(this).attr("data-toggle","false");
		$(this).text('Add to Social Calendar');
	} else {
		$(this).attr("data-toggle","true");
		$(this).text('Remove from Social Calendar');
	}
	
	// $(this).text('Remove from Social Calendar');
});