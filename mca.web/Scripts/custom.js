$(document).ready(function () {
    $('#first-row').find('th button span').addClass('glyphicon-minus');
    $('.shrow').not("[id*=first-row]").nextAll('tr').addClass('showhide');
    $('.shrow').not("[id*=first-row]").find('th button span').addClass('glyphicon-plus');

    $('.shrow').click(function () {
        $(this).nextAll('tr').toggleClass('showhide');

        if ($(this).find('th button span').hasClass('glyphicon-plus')) {
            $(this).find('th button span').removeClass('glyphicon-plus');
            $(this).find('th button span').addClass('glyphicon-minus');
        }
        else {
            $(this).find('th button span').removeClass('glyphicon-minus');
            $(this).find('th button span').addClass('glyphicon-plus');
        }

    });
});



///////// POPUP_JQUERY
$('.popTbl').each( function(){	
	var thistarget = $(this).attr('data-target');
	
    $(this).click(function(){ 
	    var toppy = $(this).offset().top;
		$('body').find('.poptable').not(thistarget).removeClass('trued');
		$(thistarget).toggleClass('trued').css('top',toppy);
    });

});


$(document).on("click", ".btn-close", function (e) {
    $('body').find('.trued').removeClass("trued");
});







