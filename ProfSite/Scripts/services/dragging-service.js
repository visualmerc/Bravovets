'use strict';

angular.module('chartsApp')
.factory('dragging', ['$rootScope', 'mouseCapture', function ($rootScope, mouseCapture) {
    var threshold = 5;
    return {
        startDrag: function (evt, config) {
            var dragging = false,
                x,
                y,
                type = evt.type,
                isTouch = (type === 'touchstart' || type === 'touchmove');

            if(isTouch) {
                x = evt.touches[0].pageX;
                y = evt.touches[0].pageY;
            } else {
                x = evt.pageX;
                y = evt.pageY;
            }
            var mouseMove = function (evt) {
                var pageX, pageY;

                if(isTouch) {
                    pageX = evt.touches[0].pageX;
                    pageY = evt.touches[0].pageY;
                } else {
                    pageX = evt.pageX;
                    pageY = evt.pageY;
                }

                if (!dragging) {
                    if (Math.abs(pageX - x) > threshold || Math.abs(pageY - y) > threshold) {
                        dragging = true;
                        if (config.dragStarted) {
                            config.dragStarted(x, y, evt);
                        }
                        if (config.dragging) {
                            config.dragging(pageX, pageY, evt);
                        }
                    }
                } else {
                    if (config.dragging) {
                        config.dragging(pageX, pageY, evt);
                    }
                    x = pageX;
                    y = pageY;
                }
            };
            var released = function () {
                if (dragging) {
                    if (config.dragEnded) {
                        config.dragEnded();
                    }
                } else {
                    if (config.clicked) {
                        config.clicked();
                    }
                }
            };
            var mouseUp = function (evt) {
                mouseCapture.release();
                evt.stopPropagation();
                evt.preventDefault();
            };
            mouseCapture.acquire(evt, {
                mouseMove: mouseMove,
                mouseUp: mouseUp,
                released: released,
            });
            evt.stopPropagation();
            evt.preventDefault();
        },
    };
}]);