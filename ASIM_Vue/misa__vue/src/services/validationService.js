import baseService from './baseService';

const validationService = {
  /**
   * Check unique code
   * Swagger: GET /api/v1/validations/check-code
   */
  checkCode(params) {
    return baseService.get('/validations/check-code', { params });
  },

  /**
   * Check formula
   * Swagger: POST /api/v1/validations/check-formula
   */
  checkFormula(payload) {
    return baseService.post('/validations/check-formula', payload);
  },
};

export default validationService;
