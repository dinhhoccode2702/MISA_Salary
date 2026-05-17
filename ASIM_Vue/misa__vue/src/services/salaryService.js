import baseService from './baseService';

const salaryService = {
  /**
   * Get salary compositions (non-paging)
   */
  getAll(params) {
    return baseService.get('/SalaryCompositions', { params });
  },

  /**
   * Get salary compositions (paging)
   * Swagger: GET /api/v1/SalaryCompositions/paging
   */
  getPaging(params) {
    return baseService.get('/SalaryCompositions/paging', { params });
  },

  /**
   * Get salary compositions (list endpoint)
   * Swagger: GET /api/v1/SalaryCompositions
   */
  getList(params) {
    return baseService.get('/SalaryCompositions', { params });
  },

  /**
   * Get a single salary composition by ID
   */
  getById(id) {
    return baseService.get(`/SalaryCompositions/${id}`);
  },

  /**
   * Create a new salary composition
   */
  create(data) {
    return baseService.post('/SalaryCompositions', data);
  },

  /**
   * Update an existing salary composition
   */
  update(id, data) {
    return baseService.put(`/SalaryCompositions/${id}`, data);
  },

  /**
   * Delete a salary composition
   */
  delete(id) {
    return baseService.delete(`/SalaryCompositions/${id}`);
  },

  /**
   * Batch delete
   * Swagger: DELETE /api/v1/SalaryCompositions/batch
   */
  deleteBatch(data) {
    // NOTE: axios supports body in DELETE via { data }
    return baseService.delete('/SalaryCompositions/batch', { data });
  },

  /**
   * Clone/Duplicate a salary composition
   * Swagger: POST /api/v1/SalaryCompositions/{id}/clone
   */
  clone(id) {
    return baseService.post(`/SalaryCompositions/${id}/clone`);
  },

  // Backward-compat alias (older code calls duplicate)
  duplicate(id) {
    return this.clone(id);
  },

  /**
   * Update status
   * Swagger: PATCH /api/v1/SalaryCompositions/{id}/status
   */
  updateStatus(id, data) {
    return baseService.patch(`/SalaryCompositions/${id}/status`, data);
  },

  /**
   * Bulk import
   * Swagger: POST /api/v1/SalaryCompositions/bulk-import
   */
  bulkImport(data) {
    return baseService.post('/SalaryCompositions/bulk-import', data);
  },
};

export default salaryService;
