var startVal = 58;

$(function(){

	$('#slider').slider({
step: 1,
min: 15,
value: startVal,
max: 143,
slide: function(event, ui) {
		  $('#content-grid li').css('font-size',ui.value+"px");
	  }
	});	
});

function update_slider(){
	var offset = $('.ui-slider-handle').offset();
	var value = $('#slider').slider('option', 'value');
}	

$(document).ready(function(){
	// Open all links in new window
	$('a[href^="http://"]')	.attr({ target: "_blank" });

	$('#content-grid li').css('font-size',startVal+"px");
	initializeGrid();
});


function initializeGrid() {
	$("#content-grid li block").each(function() {
		var width = $(this).width() / 100 + "em";
		var height = $(this).height() / 100 + "em";
		$(this).css("width", width);
		$(this).css("height", height );
	});
}
