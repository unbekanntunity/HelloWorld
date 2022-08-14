import React, { Component } from 'react';
import { Link, useNavigate } from 'react-router-dom';

import InputField from '../components/InputField/InputField';
import RankingBox from '../components/RankingBox';
import { Post } from '../components/Post';
import { Dialog, ReportDialog } from '../components/Dialog';

import profile from '../images/profile.png';

import { handleUpdateRating, sendJSONRequest } from '../requestFuncs';

import './Home.css';

class Home extends Component {
    state = {
        posts: {
            
        },
        topDiscussion: [
            {
                name: "Hello",
                tags: [
                    "C#",
                    "F#",
                ]
            }
        ],
        topProjects: [
            {
                name: "Hello World",
                tags: [
                    "C#",
                    "Social",
                ]
            }
        ],
        inputFieldDefaults: [
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
        ],
       
        showShareDialog: false,
        showReportDialog: false,
    }

    componentDidMount() {
        this.getPosts();
    }

    getPosts = () => {
        sendJSONRequest("GET", "/post/get_all", undefined, this.props.tokens.token)
            .then(response => {
            this.setState({ posts: response.data })
        }, error => {
            console.log(error);
            this.props.onError(error.message)
        });
    }

    handleCreatorInfos = (index) => {
        let newPosts = this.state.posts;

        sendJSONRequest("GET", `/user/get_minimal/${this.state.posts[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newPosts[index].creatorImage = response.data.imageUrl;
                newPosts[index].creatorName = response.data.userName;
                this.setState({ posts: newPosts })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            });
    }

    handleOnNotificationIconClick = () => {

    }

    handleShare = () => {
         this.setState({ showShareDialog: !this.state.showShareDialog });
    }

    handleSuccessRating = (index, response) => {
        let newPosts = this.state.posts;
        newPosts[index].usersLikedIds = response.data.usersLikedIds;
        this.setState({ posts: newPosts })
    }

    render() {
        console.log(this.state.posts)
        return (
            <div style={{height: "100%"} }>
                <div className="home-body">
                    <div className="footer-box">
                        <Link className="link" to="/impressum"><p>Impressum</p></Link >
                        <Link className="link" to="/policies"><p>Policies</p></Link >
                        <Link className="link" to="/support"><p>Support</p></Link >
                    </div>
                    <div className="center-vertical column">
                        {
                            Array.from(this.state.posts).map((item, index) =>
                                <Post key={index} keyProp={index} imageHeight={200} item={item}
                                    sessionUserId={this.props.sessionUserId} onShareClick={() => this.setState({ showShareDialog: true })}
                                    onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })} width={400} creatorId={item.creatorId}
                                    onLike={(index) => handleUpdateRating(item.id, "post", this.props.tokens.token, this.props.onError, (response) => this.handleSuccessRating(index, response))} />
                            )
                        }
                    </div>
                    <div>
                        <div className="ranking-box-wrapper">
                            <RankingBox width="200px" height="250px" title="Top discussion" items={this.state.topDiscussion} />
                        </div>
                        <div className="ranking-box-wrapper">
                            <RankingBox width="200px" height="250px" title="Top discussion" items={this.state.topProjects} />
                        </div>
                    </div>
                </div>
                {
                    this.state.showShareDialog && 
                    <Dialog title="Share" onBackClick={this.handleShare} paddingX="10px" paddingY="20px"
                            height="fit-content" width="400px"
                            rightText="Other" rightTextColor="#0079D3">
                        <InputField design="m2" defaults={this.state.inputFieldDefaults} />
                    </Dialog>
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
            </div>
        );
    }
}

export default Home;