import baseService from './baseService';

const gridConfigService = {
  getGridConfigs(params) {
    return baseService.get('/grid-configs', { params });
  },
  saveGridConfigs(payload) {
    return baseService.put('/grid-configs', payload);
  },
};

export default gridConfigService;
