import baseService from './baseService';

const dictionaryService = {
  getComponentTypes() {
    return baseService.get('/dictionaries/component-types');
  },
  getNatureTypes() {
    return baseService.get('/dictionaries/nature-types');
  },
  getDataTypes() {
    return baseService.get('/dictionaries/data-types');
  },
};

export default dictionaryService;
