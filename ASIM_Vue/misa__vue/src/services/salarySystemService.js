import baseService from './baseService'
const salarySystemService = {
  getAll() {
    return baseService.get('/salary-systems');
  }
};
export default salarySystemService;