(function () {
    angular
    .module('app')
    .controller('PersonController', PersonController);

    function PersonController($scope, $log, $http) {
        var vm = this;
        vm.persons = [];

        this.click = function(documentsLink) {
            $log.info("clicked");
        	$http.get('/api/Person?documentsLink=' + encodeURIComponent(documentsLink)).
                success(function (data, status, headers, config) {
                    $log.info("got persons: " + data);
                    vm.persons = data;
                }).
                error(function (data, status, headers, config) {
                    $log.error("get persons failed");
                });
        };
    }
})();