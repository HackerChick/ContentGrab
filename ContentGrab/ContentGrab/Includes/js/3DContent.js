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

	$('#current_value').text('Value is '+value).css({top:offset.top });
	$('#current_value').fadeIn();
}

$(document).ready(function () {
	// Open all links in new window
    $('a[href^="http://"]').attr({ target: "_blank" });

    // Auto-size content
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

// Show/Hide Based on Selected Tags
function hideAll() {
    $('li.contentItem').hide();
    $('li.picklistItem').show();
}
function showAll() {
    $('li.contentItem').show();
}
$(document).ready(function () {
    $('a.filter').click(function () {
        hideAll();
        var classToShow = $(this).attr('id');
        $('li.' + classToShow).show();
    });
    $('a.show').click(function () {
        showAll();
    });
});

// Move content into/out of pick list
$(document).ready(function () {
    $('a.select').click(function () {
        var divToMove = "#item_" + $(this).attr('id');

        var newLocation = "";
        if ($(divToMove).hasClass("picklistItem")) {
            newLocation = "#content-grid";
            $(divToMove).appendTo(newLocation);
            $(divToMove).removeClass("picklistItem");
            $(divToMove).height("5em");                             // based on css .grid li
            $(divToMove).width("3.5em");                            // based on css .grid li
            $('#content-grid li').css('font-size', $('#content-grid li').css('font-size'));
        } else {
            newLocation = "#picklist-grid";
            $(divToMove).appendTo(newLocation);
            $(divToMove).height($("div.picklist").height());
            $(divToMove).width($("div.picklist").height() / 1.4);   // based on css .grid li width:height ratio
            $('#picklist-grid li').css('font-size', "5em");
            $(divToMove).addClass("picklistItem");
        }

    });
}); 