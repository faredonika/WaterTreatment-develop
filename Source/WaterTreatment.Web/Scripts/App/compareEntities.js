define([], function () {
    return function compareEntities(arr1, arr2) {
        if (arr1.length !== arr2.length)
            return false;
        for (var i = arr1.length; i--;) {

            if (arr1[i].initialState.id !== arr2[i].initialState.id)
                return false;
        }

        return true;
    };
})