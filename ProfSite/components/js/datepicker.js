// // placeholder for custom dropdown js
$(function() {
    $.fn.DatePicker = function(objectName, settings) {
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
            // TODO: Replace with country-specific formatting
            _format = "M dd, yyyy";
            _initialDate = $me.find('.input-group.date input').val();

            _init = function() {
                debug("starting datepicker:" + $me.attr('data-language'));
                if ($me.attr('data-language')) {
                	_language = $me.attr('data-language');
                    console.log("SETTING LANGUAGE:" + $me.attr('data-language'));
                }
                $me.find('.date').datepicker({
	            	todayBtn: true,
	            	todayHighlight: true,
	            	language: _language,
                    format: _format
	            });
                // If there is a date in the input box, set the internal date of the datepicker
                if (_initialDate) {
                    $me.find('.date').datepicker('setDate', _initialDate);
                }
                
            };
            return _init();
        });
        return this;
    };

});

initializeComponents("datepicker");

// Placeholder for datepicker js
// $('.datepicker-container.en .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true
// });
// $('.datepicker-container.de .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true,
// 	language: "de"
// });
// $('.datepicker-container.es .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true,
// 	language: "es"
// });
// $('.datepicker-container.fr .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true,
// 	language: "fr"
// });
// $('.datepicker-container.nl .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true,
// 	language: "nl"
// });
// $('.datepicker-container.it .input-group.date').datepicker({
// 	todayBtn: true,
// 	todayHighlight: true,
// 	language: "it"
// });