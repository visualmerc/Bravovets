// javascript for timepicker 
$(function() {
    $.fn.TweetLength = function(objectName, settings) {
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
            _tweetField = null;
            _tweetFieldID = null;
            _count = 0;
            _currentCountElement = null;

            _selectText = function(start, end) {
                var e = document.getElementById(_tweetFieldID); // I don't know why... but $(this) don't want to work today :-/
                console.log(start + "," + end);
                if (!e) return;
                else if (e.setSelectionRange) { e.focus(); e.setSelectionRange(start, end); } /* WebKit */ 
                else if (e.createTextRange) { var range = e.createTextRange(); range.collapse(true); range.moveEnd('character', end); range.moveStart('character', start); range.select(); } /* IE */
                else if (e.selectionStart) { e.selectionStart = start; e.selectionEnd = end; }
            }

            _tweetChange = function() {
                _count = $(this).val().length;
                _currentCountElement.text(_count);
                if (_count > 140) {
                    $me.addClass('error');
                    tweet = $(this).val();
                } else {
                    $me.removeClass('error');
                }
            }

            _init = function() {
                debug("starting tweetlength");
                _currentCountElement = $me.find('.current-count');
                _currentCountElement.text('0');
                _tweetFieldID = $me.attr('data-tweetfield');
                if (_tweetFieldID) {
                    _tweetField = $('#' + $me.attr('data-tweetfield'));
                    _tweetField.bind('input propertychange',_tweetChange);
                }
            };
            return _init();
        });
        return this;
    };

});

initializeComponents("tweetlength");
