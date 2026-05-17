import baseService from './baseService';

const endpoint = '/organizations';

const organizationService = {
  /**
   * Lấy danh sách cơ cấu tổ chức
   * @param {Object} params - Các tham số query (keyword, limit, offset,...)
   * @returns Promise
   */
  getAll(params) {
    return baseService.get(endpoint, { params });
  },

  /**
   * Lấy tổ chức theo ID
   * @param {String|Number} id 
   * @returns Promise
   */
  getById(id) {
    return baseService.get(`${endpoint}/${id}`);
  }
};

export default organizationService;
