
//Detectizr.detect({ detectScreen: false });


//function to do simple string formatting
String.prototype.format = String.prototype.f = function () {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};


//**************************************************************************************************************
(function ($) {
    $.extend($.ui, { BravoVetDropDown: {} });

    function BravoVetDropDown() {
        this.widget = null;

    }

    $.extend(BravoVetDropDown.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;

            $('select').each(function () {
                var $this = $(this), numberOfOptions = $(this).children('option').length;

                $this.addClass('select-hidden');
                $this.wrap('<div class="select"></div>');
                $this.after('<div class="select-styled"></div>');

                var $styledSelect = $this.next('div.select-styled');
                $styledSelect.text($this.children('option').eq(0).text());

                var $list = $('<ul />', {
                    'class': 'select-options'
                }).insertAfter($styledSelect);

                for (var i = 0; i < numberOfOptions; i++) {
                    var val = $this.children('option').eq(i).val();
                    if (val !== "") {
                        $('<li />', {
                            text: $this.children('option').eq(i).text(),
                            rel: val
                        }).appendTo($list);
                    }
                }

                var $listItems = $list.children('li');

                $styledSelect.click(function (e) {
                    e.stopPropagation();
                    $('div.select-styled.active').each(function () {
                        $(this).removeClass('active').next('ul.select-options').hide();
                    });
                    $(this).toggleClass('active').next('ul.select-options').toggle();
                });

                $listItems.click(function (e) {
                    e.stopPropagation();
                    $styledSelect.text($(this).text()).removeClass('active');
                    var val = $(this).attr('rel');
                    
                    $this.val(val).trigger('change');
                    $list.hide();
                    //console.log($this.val());
                });

                $(document).click(function () {
                    $styledSelect.removeClass('active');
                    $list.hide();
                });

            });

            return this;
        },
    });
    $.fn.bravoVetDropDown = function (options) {
        var control = new BravoVetDropDown();
        control.init(options, this);
        return control;
    };
})(jQuery);