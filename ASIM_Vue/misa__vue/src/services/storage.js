import candidateData from '../assets/candidate.json';

const KEY = 'Candidate';

// load data từ json
export async function setCandidateToStorage() {
    try {
        const mappedData = candidateData.map((candidate, index) => {
            return {
                id: Date.now() + '_' + index, // ID duy nhất kết hợp với index để không bị trùng lặp
                ...candidate
            }
        })
        localStorage.setItem(KEY, JSON.stringify(mappedData));
        return mappedData;
    } catch (error) {
        console.error('Error setting candidate data:', error);
        throw error;
    }
}

//get data from storage
export async function getCandidateFromStorage() {
    try {
        const storedData = localStorage.getItem(KEY);
        if (storedData) {
            return JSON.parse(storedData);
        }
        // fallback to fetching the asset and persisting it
        return await setCandidateToStorage();
    } catch (e) {
        console.error('Error reading candidate from storage:', e);
        return [];
    }
}

//clear
export function clearCandidateStorage() {
    try {
        localStorage.removeItem(KEY);
    } catch (e) {
        console.error('Error clearing candidate storage:', e);
    }
}

// save data to storage
export async function saveDataToStorage(candidate) {
    try {
        localStorage.setItem(KEY,JSON.stringify(candidate));
    }catch (error) {
        console.error('Error saving candidate data:', error);
        throw error;
    }
}

// delete candidate from storage
export async function deleteCandidateFromStorage(id) {
    try {
        const candidates = await getCandidateFromStorage();
        const updatedCandidates = candidates.filter(c => c.id !== id);
        await saveDataToStorage(updatedCandidates);
    } catch (error) {
        console.error('Error deleting candidate:', error);
        throw error;
    }
}