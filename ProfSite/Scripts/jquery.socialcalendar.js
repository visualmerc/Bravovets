
//socialtips
(function ($) {
    $.extend($.ui, { SocialCalendar: {} });

    function SocialCalendar() {
        this.widget = null;
        this.calendarWidget = null;
    }

    $.extend(SocialCalendar.prototype, {
        init: function (options, control) {
            this.widget = control;

            var defaults = {};

            var allOptions = $.extend(defaults, options);

            //setup calendar
            this._setupCalendar();
            //setup buttons
        },
        _setupCalendar: function (options) {
            var cal = this.widget.find(".calendar");
            var _language = '';
            var self = this;

            var _options = {
                events_source: '/share/SocialCalendarPosts'
                //view: 'month',
                //tmpl_path: 'tmpls/',
              //  tmpl_cache: false,
                //day: '2013-03-12',
               // language: _language,
              //  modal: '#events-modal',
                //onAfterEventsLoad: function (events) {
                //    //if (!events) {
                //    //    return;
                //    //}
                //    //var list = self.widget.find('#eventlist');
                //    //list.html('');

                //    //$.each(events, function (key, val) {
                //    //    $(document.createElement('li'))
                //    //        .html('<a href="' + val.url + '">' + val.title + '</a>')
                //    //        .addClass(val.network)
                //    //        .appendTo(list);
                //    //});
                //},
                //onAfterViewLoad: function (view) {

                //    //    self.widget.find('.calendar-title').text(this.getTitle());
                //    //    self.widget.find('.btn-group button').removeClass('active');
                //    //    self.widget.find('button[data-calendar-view="' + view + '"]').addClass('active');
                //},
                //classes: {
                //    months: {
                //        general: 'label'
                //    }
                //}
            };



            this.calendarWidget = $(cal).calendar(_options);

        }

    });

    $.fn.socialCalendar = function (options) {
        var control = new SocialCalendar();
        control.init(options, this);
        return control;
    };
})(jQuery);




//$(function () {
//    $.fn.SocialCalendar = function (objectName, settings) {
//        var $parent, config;
//        this.settings = settings;

//        $parent = $(this);
//        if (typeof config === "undefined" || config === null) {
//            config = {};
//        }

//        config.myName = objectName;
//        if (this.settings != null) {
//            jQuery.extend(config, this.settings);
//        }

//        this.each(function (index) {
//            var $me;
//            $me = $(this);
//            var _language = '';

//            if ($me.attr("data-language")) {
//                debug("Setting calendar language: " + $me.attr("data-language"));
//                _language = $me.attr("data-language");
//            }

//            var _options = {
//                events_source: 'calendarposts.json',
//                view: 'month',
//                tmpl_path: 'tmpls/',
//                tmpl_cache: false,
//                //day: '2013-03-12',
//                language: _language,
//                modal: '#events-modal',
//                onAfterEventsLoad: function (events) {
//                    if (!events) {
//                        return;
//                    }
//                    var list = $me.find('#eventlist');
//                    list.html('');

//                    $.each(events, function (key, val) {
//                        $(document.createElement('li'))
//							.html('<a href="' + val.url + '">' + val.title + '</a>')
//							.addClass(val.network)
//							.appendTo(list);
//                    });
//                },
//                onAfterViewLoad: function (view) {
//                    console.log("updating calendar view");
//                    $me.find('.calendar-title').text(this.getTitle());
//                    $me.find('.btn-group button').removeClass('active');
//                    $me.find('button[data-calendar-view="' + view + '"]').addClass('active');
//                },
//                classes: {
//                    months: {
//                        general: 'label'
//                    }
//                }
//            };

//            _setupButtons = function() {
//                $me.find('.btn-group button[data-calendar-nav]').each(function() {
//                    var $this = $(this);
//                    $this.click(function() {
//                        calendar.navigate($this.data('calendar-nav'));
//                    });
//                });

//                $me.find('.btn-group button[data-calendar-view]').each(function() {
//                    var $this = $(this);
//                    $this.click(function() {
//                        calendar.view($this.data('calendar-view'));
//                    });
//                });
//            };

//            _setupCalendar = function() {
//                calendar = $me.find('.calendar').calendar(_options);
//            };

//            _init = function () {
//                _setupCalendar();
//                _setupButtons();
//            };
//            return _init();
//        });
//        return this;
//    };

//$.fn.socialCalendar = function (options) {
//    var control = new SocialCalendar();
//    control._init();
//    return control;
//};

//});
