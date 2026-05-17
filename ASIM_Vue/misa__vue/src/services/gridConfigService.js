import baseService from './baseService';

const gridConfigService = {
  getGridConfigs(params) {
    return baseService.get('/grid-configs', { params });
  },
  saveGridConfigs(payload, params) {
    return baseService.put('/grid-configs', payload, { params });
  },
};

export default gridConfigService;
