import { afterEach, describe, expect, it, vi } from "vitest";
import { search } from "./searchService";

describe("search", () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    it("builds the search request correctly", async () => {
        const mockResponse = {
            items: [],
            page: 1,
            pageSize: 25,
            totalCount: 0,
        };

        const fetchMock = vi.fn().mockResolvedValue({
            json: vi.fn().mockResolvedValue(mockResponse),
        });

        vi.stubGlobal("fetch", fetchMock);

        await search("ash vampire", 1, 25);

        expect(fetchMock).toHaveBeenCalledWith(
            "http://localhost:5080/api/search?query=ash%20vampire&page=1&pageSize=25"
        );
    });

    it("returns the parsed API response", async () => {
        const mockResponse = {
            items: [],
            page: 2,
            pageSize: 25,
            totalCount: 50,
        };

        const fetchMock = vi.fn().mockResolvedValue({
            json: vi.fn().mockResolvedValue(mockResponse),
        });

        vi.stubGlobal("fetch", fetchMock);

        const result = await search("vivec", 2, 25);

        expect(result).toEqual(mockResponse);
    });
});