// javascript for timepicker 
$(function() {
    $.fn.TimePicker = function(objectName, settings) {
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
            _language = "en";

            _init = function() {
                debug("starting timepicker");
                if ($me.attr('data-language')) {
                	_language = $me.attr('data-language');
                }
                $me.find('.bootstrap-timepicker input').timepicker({showMinutes: false, lowerCaseMeridian: true, showMeridian: false});
            };
            return _init();
        });
        return this;
    };

});

initializeComponents("timepicker");
