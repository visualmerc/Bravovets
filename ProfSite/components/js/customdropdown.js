// // placeholder for custom dropdown js
$(function() {
    $.fn.CustomDropdown = function(objectName, settings) {
        var $parent, config;
        this.settings = settings;

        $parent = $(this);
        if (typeof config === "undefined" || config === null) {
            config = {};
        }

        config.myName = objectName;
        if (this.settings != null) {
            jQuery.extend(config, this.settings);
        }

        this.each(function(index) {
            var $me;
            $me = $(this);

            _init = function() {
                debug("starting custom dropdown");
                $('select').each(function(){
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
                        $('<li />', {
                            text: $this.children('option').eq(i).text(),
                            rel: $this.children('option').eq(i).val()
                        }).appendTo($list);
                    }
                  
                    var $listItems = $list.children('li');
                  
                    $styledSelect.click(function(e) {
                        e.stopPropagation();
                        $('div.select-styled.active').each(function(){
                            $(this).removeClass('active').next('ul.select-options').hide();
                        });
                        $(this).toggleClass('active').next('ul.select-options').toggle();
                    });
                  
                    $listItems.click(function(e) {
                        e.stopPropagation();
                        $styledSelect.text($(this).text()).removeClass('active');
                        $this.val($(this).attr('rel'));
                        $list.hide();
                        //console.log($this.val());
                    });
                  
                    $(document).click(function() {
                        $styledSelect.removeClass('active');
                        $list.hide();
                    });

                });
            };
            return _init();
        });
        return this;
    };

});

initializeComponents("customdropdown");