'use strict';

$(function () {
    //find "Read More" text
    var itemWidth;

    //set up initial slides
    $('.item.active').find('.content-wrapper').each(function (i, item) {
        itemWidth = $(item).width();
        if (item.scrollHeight >= 400 || $(item).find(".attachment-wrapper").length > 0) {
            adjustHeight($(item));
        }
    });

    //fix slides as they animate
    $('.carousel').on('slide.bs.carousel', function (e) {

        var $aContent;

        if (e.direction === 'left') {
            $aContent = $(this).find('.active').next().find('.content-wrapper');
        } else {
            $aContent = $(this).find('.active').prev('.item').find('.content-wrapper');
            if ($aContent.length === 0) {
                $aContent = $(this).find('.item').last().find('.content-wrapper');
            }
        }

        setTimeout(function () {

            if ($aContent.prop('scrollHeight') >= 400 || $aContent.find(".attachment-wrapper").length > 0) {
                adjustHeight($aContent);
            }
        }, 10);
    });


    function adjustHeight($aContent) {
        var $p = $aContent.find('.content-text'),
            i = 0,
            beginHeight = (itemWidth < 270) ? 180 : 130,
            maxHeight;

        //adjust copy
        $p.data('original-text', $p.html());
        maxHeight = beginHeight - ($aContent.find('h3').height() - 17);

        $p.find(".attachment-wrapper").remove();

        while ($p.outerHeight() > maxHeight && i < 200) {
            $p.text(shortenText);
            i++;
        }
       
        //insert read more button
        var readmore = $aContent.find(".readmore").show();

        readmore.on('click touch-start', function (e) {
            e.preventDefault();
            openReadMoreModal($aContent);
        });
    }

    function shortenText(index, text) {
         return text.replace(/\w*\s*(\S)*(\W)*$/, '...');
    }

    function openReadMoreModal($aContent) {
      
        var content = '<ul class="bravecto-content syndicatedcontent-socialtips"><li class="panel-diagonalbottom">';
        content += $aContent.find('.content-text').data('original-text');
        content += '</li></ul>';

        $('body').append('<div id="readMoreModal" class="modal fade in" tabindex="-1" role="dialog"> <div class="modal-dialog"> <div class="modal-content bravecto-content"> <div class="modal-header"> <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button> <h4 class="modal-title">' + $aContent.find('h3').html() + '</h4> </div><hr class="diagonal plus"><div class="modal-body"><p>' + $aContent.find('.content-text').data('original-text') + '</p></div> </div> </div> </div>');

        //kill modal on close
        $('#readMoreModal').modal().on('hidden.bs.modal', function (e) {

            $('#readMoreModal').detach();
        });
    }
});