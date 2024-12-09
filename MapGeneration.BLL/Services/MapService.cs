using AutoMapper;
using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.DAL.EF;
using MapGeneration.DAL.EF.Repositories;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.BLL.Services;

public class MapService : IMapService
{
    private readonly IRepository<UserEntity> _userRepository;
    private readonly IMapRepository _mapRepository;
    //private readonly IRepository<LikeEntity> _likeRepository;
    //private readonly IRepository<CommentEntity> _commentRepository;

    private readonly IMapper _mapper;

    public MapService(
        IRepository<UserEntity> userRepository,
        IMapRepository mapRepository,
        IRepository<LikeEntity> likeRepository,
        IRepository<CommentEntity> commentRepository,
        IMapper mapper
    )
    {
        _userRepository = userRepository;
        _mapRepository = mapRepository;
        //_likeRepository = likeRepository;
        //_commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<MapModel> Create(MapModel mapModel)
    {
        UserEntity user = await _userRepository.FindAsync(mapModel.Author.Id);
        MapEntity mapEntity = _mapper.Map<MapEntity>(mapModel);
        mapEntity.Author = user;
        mapEntity.DateTime = DateTime.UtcNow;

        MapEntity createdEntity = await _mapRepository.CreateAsync(mapEntity);
        MapModel createdMap = _mapper.Map<MapModel>(createdEntity);
        return createdMap;
    }

    public async Task<MapModel> FindAsync(Guid id)
    {
        return _mapper.Map<MapModel>(await _mapRepository.FindAsync(id));
    }

    public Task<MapModel> UpdateAsync(MapModel mapModel)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MapModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField,
        SortingDirection direction
        )
    {
        bool ascending = direction == SortingDirection.ASC;
        IEnumerable<MapModel> result =
            _mapper.Map<IEnumerable<MapModel>>(
                _mapRepository.GetPagedAsync(page, pageSize, sortField, ascending));
        return result;
    }

    public async Task<IEnumerable<MapModel>> GetPagedByUserAsync(
        int page,
        int pageSize,
        string sortField,
        SortingDirection direction,
        UserModel userModel
    )
    {
        bool ascending = direction == SortingDirection.ASC;
        UserEntity userEntity = _mapper.Map<UserEntity>(userModel);
        IEnumerable<MapModel> result = _mapper.Map<IEnumerable<MapModel>>(_mapRepository.GetPagedByUserAsync(page, pageSize, sortField, ascending, userEntity));
        return result;
    }

    public int GetPagesCount(int pageSize)
    {
        return (int)Math.Ceiling((decimal)(_mapRepository.Count() / pageSize));
    }


    public async Task<IEnumerable<MapModel>> GetPagedAsync(
        int page,
        int pageSize,
        string sortField
    )
    {
        bool ascending = true;
        IEnumerable<MapModel> result =
            _mapper.Map<IEnumerable<MapModel>>(
                _mapRepository.GetPagedAsync(page, pageSize, sortField, ascending));
        return result;
    }
}