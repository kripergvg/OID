(function () {
    'use strict';

   var appModule = angular.module('app',[]);

    appModule.controller('ObjectListController',
        function($scope) {
            $scope.objects = objectList;
        });
})();
