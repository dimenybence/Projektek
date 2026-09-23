interface PaginationProps {
  currentPage: number
  totalPages: number
  total: number
  pageSize: number
  onPageChange: (page: number) => void
  disabled?: boolean
}

export function Pagination({
  currentPage,
  totalPages,
  total,
  pageSize,
  onPageChange,
  disabled = false,
}: PaginationProps) {
  if (totalPages <= 1) return null

  const from = total === 0 ? 0 : (currentPage - 1) * pageSize + 1
  const to = Math.min(currentPage * pageSize, total)

  return (
    <nav className="pagination" aria-label="Lapozás">
      <p className="muted small pagination-summary">
        {from}–{to} / {total} kvíz
      </p>
      <div className="button-row">
        <button
          type="button"
          className="btn secondary small"
          disabled={disabled || currentPage <= 1}
          onClick={() => onPageChange(currentPage - 1)}
        >
          Előző
        </button>
        <span className="pagination-page">
          {currentPage}. / {totalPages} oldal
        </span>
        <button
          type="button"
          className="btn secondary small"
          disabled={disabled || currentPage >= totalPages}
          onClick={() => onPageChange(currentPage + 1)}
        >
          Következő
        </button>
      </div>
    </nav>
  )
}
